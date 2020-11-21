using System;
using ClientIntegrator.Common.Helpers;
using ClientIntegrator.Common.Services;
using ClientIntegrator.DataAccess;
using ClientIntegrator.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClientIntegrator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static ClientIntegratorDbContext ClientIntegratorDbContext { get; private set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("ClientIntegratorDb");

            services.AddDbContext<ClientIntegratorDbContext>(options =>
                options.UseNpgsql(connectionString,
                x => x.MigrationsHistoryTable("migration_history", "admin")));

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 6;
            });

            //Dependency Injection
            ConfigureDi(services);

            services.AddDefaultIdentity<PortalUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<PortalRole>()
                .AddEntityFrameworkStores<ClientIntegratorDbContext>();

            services.AddDataProtection()
            .PersistKeysToDbContext<ClientIntegratorDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorPagesOptions(options =>
            {
                // set home as default landing page
                options.Conventions.AddPageRoute("/Configuration/Index", "");
            }); ;

            services.AddControllers(config =>
            {
                // using Microsoft.AspNetCore.Mvc.Authorization;
                // using Microsoft.AspNetCore.Authorization;
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        private void ConfigureDi(IServiceCollection services)
        {
            //General
            services.AddSingleton(Configuration);

            //External services
            services.AddTransient<ISmtpEmailService, SmtpEmailService>();
            services.AddTransient<ILayoutService, LayoutService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });


            using (var scope = app.ApplicationServices.CreateScope())
            {
                //initialize & seed database
                using (var dbContext = scope.ServiceProvider.GetService<ClientIntegratorDbContext>())
                {
                    ClientIntegratorDbContext = dbContext;
                    var connectionString = Configuration.GetConnectionString("ClientIntegratorDb");
                    if (connectionString != "InMemory")
                    {
                        //initialize & migrate to latest version
                        dbContext.Database.Migrate();
                    }

                    var roleManager = scope.ServiceProvider.GetService<RoleManager<PortalRole>>();
                    var userManager = scope.ServiceProvider.GetService<UserManager<PortalUser>>();
                    var userStore = scope.ServiceProvider.GetService<IUserStore<PortalUser>>();

                    if (!userManager.SupportsUserEmail)
                    {
                        throw new NotSupportedException("The UI requires a user store with email support.");
                    }

                    IUserEmailStore<PortalUser> emailStore = (IUserEmailStore<PortalUser>)userStore;

                    if (!env.IsProduction())
                    {
                        DbSeeder.Seed(dbContext, emailStore, userStore, userManager, roleManager, Configuration).Wait();
                    }
                    else
                    {
                        DbSeeder.SeedProduction(dbContext, emailStore, userStore, userManager, roleManager, Configuration).Wait();
                    }
                }
            }
        }
    }
}
