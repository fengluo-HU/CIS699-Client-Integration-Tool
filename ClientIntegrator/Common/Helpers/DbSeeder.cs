using ClientIntegrator.DataAccess;
using ClientIntegrator.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ClientIntegrator.Common.Helpers
{
    public class DbSeeder
    {
        public static async Task Seed(ClientIntegratorDbContext dbContext,
            IUserEmailStore<PortalUser> emailStore,
            IUserStore<PortalUser> userStore, UserManager<PortalUser> userManager,
            RoleManager<PortalRole> roleManager, IConfiguration configuration)
        {
            //seed organization
            var organization = await dbContext.Organizations.FirstOrDefaultAsync();
            if (organization == null)
            {
                organization = new Organization
                {
                    DisplayName = "Test Org",
                    CreateAt = DateTime.UtcNow,
                    CreateBy = 1,
                    ClientId = "test clientId",
                    ClientSecret = "test clientSecret",
                    ShowUsersAllFiles = true
                };

                dbContext.Organizations.Add(organization);
                dbContext.Organizations.Add(new Organization
                {
                    DisplayName = "second Test Org",
                    CreateAt = DateTime.UtcNow,
                    CreateBy = 1,
                    ClientId = "second test clientId",
                    ClientSecret = "second test clientSecret",
                    ShowUsersAllFiles = true
                });

                dbContext.Organizations.Add(new Organization
                {
                    DisplayName = "another Test Org",
                    CreateAt = DateTime.UtcNow,
                    CreateBy = 1,
                    ClientId = "another test clientId",
                    ClientSecret = "another test clientSecret",
                    ShowUsersAllFiles = true
                });
                await dbContext.SaveChangesAsync();
            }

            //seed configurations
            var record = await dbContext.Configurations.FirstOrDefaultAsync();
            if (record == null)
            {
                await dbContext.AddAsync(new DataAccess.Models.Configuration()
                {
                    Value = "4",
                    Notes = "Allowed thread count for multi-threading",
                    OrganizationId = organization.Id,
                    ConfigurationDict = new ConfigurationDict()
                    {
                        Notes = "Default allowed thread count for multi-threading",
                        DefaultValue = "4",
                        Key = "ThreadCount",
                        CreateAt = DateTime.UtcNow,
                        CreateBy = 1,
                        IsActive = true,
                        IsEncrypted = false
                    },
                    CreateBy = 1,
                    CreateAt = DateTime.UtcNow
                });

                await dbContext.AddAsync(new DataAccess.Models.Configuration()
                {
                    Value = "fluo@my.harrisburg.edu",
                    Notes = "Notification email receiver",
                    OrganizationId = organization.Id,
                    ConfigurationDict = new ConfigurationDict()
                    {
                        Notes = "Default notification email receiver",
                        DefaultValue = "fluo@my.harrisburg.edu",
                        Key = "NotificationEmail",
                        CreateAt = DateTime.UtcNow,
                        CreateBy = 1,
                        IsActive = true,
                        IsEncrypted = false
                    },
                    CreateBy = 1,
                    CreateAt = DateTime.UtcNow
                });
                await dbContext.SaveChangesAsync();
            }

            //seed roles
            bool userRoleExists = await roleManager.RoleExistsAsync("User");
            if (!userRoleExists)
            {
                await roleManager.CreateAsync(new PortalRole("User"));
            }

            bool superUserRoleExists = await roleManager.RoleExistsAsync("SuperUser");
            if (!superUserRoleExists)
            {
                await roleManager.CreateAsync(new PortalRole("SuperUser"));
            }

            bool adminRoleExists = await roleManager.RoleExistsAsync("Admin");
            if (!adminRoleExists)
            {
                await roleManager.CreateAsync(new PortalRole("Admin"));
            }

            var superUserEmails = new[] {
                            "fluo@my.harrisburg.edu",
                            "test.superUser@my.harrisburg.edu",
                            "AQureshi@harrisburgU.edu"
                        };

            var adminEmails = new[] {
                            "fluo@my.harrisburg.edu",
                            "test.admin@my.harrisburg.edu",
                            "AQureshi@harrisburgU.edu"
                        };


            var userEmails = new[] {
                            "test.user@my.harrisburg.edu",
                        };

            var password = configuration.GetValue<string>("Password");


            foreach (var email in superUserEmails)
            {
                var exists = await dbContext.Users.AnyAsync(x => x.UserName == email, CancellationToken.None);

                if (!exists)
                {
                    var user = new PortalUser();
                    user.IsActive = true;
                    user.Email = email;
                    user.UserName = email;

                    var result = await userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        var userId = await userManager.GetUserIdAsync(user);
                        await userManager.AddToRoleAsync(user, "SuperUser");
                        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        await userManager.ConfirmEmailAsync(user, code);
                    }
                }
            }

            foreach (var email in adminEmails)
            {
                var exists = await dbContext.Users.AnyAsync(x => x.UserName == email, CancellationToken.None);

                if (!exists)
                {
                    var user = new PortalUser();
                    user.IsActive = true;
                    user.OrganizationId = organization.Id;
                    user.Email = email;
                    user.UserName = email;

                    var result = await userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        var userId = await userManager.GetUserIdAsync(user);
                        await userManager.AddToRoleAsync(user, "Admin");
                        await userManager.AddClaimAsync(user, new Claim("OrganizationId",
                            user.OrganizationId.ToString()));
                        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        await userManager.ConfirmEmailAsync(user, code);
                    }
                }
            }

            foreach (var email in userEmails)
            {
                var exists = await dbContext.Users.AnyAsync(x => x.UserName == email, CancellationToken.None);

                if (!exists)
                {
                    var user = new PortalUser();
                    user.IsActive = true;
                    user.OrganizationId = organization.Id;
                    user.Email = email;
                    user.UserName = email;

                    var result = await userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        var userId = await userManager.GetUserIdAsync(user);
                        await userManager.AddToRoleAsync(user, "User");
                        await userManager.AddClaimAsync(user, new Claim("OrganizationId",
                            user.OrganizationId.ToString()));
                        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        await userManager.ConfirmEmailAsync(user, code);
                    }
                }
            }
        }

        public static async Task SeedProduction(ClientIntegratorDbContext dbContext, IUserEmailStore<PortalUser> emailStore, IUserStore<PortalUser> userStore, UserManager<PortalUser> userManager, RoleManager<PortalRole> roleManager, IConfiguration configuration)
        {
            var password = configuration.GetValue<string>("Password");
            //seed roles
            bool userRoleExists = await roleManager.RoleExistsAsync("User");
            if (!userRoleExists)
            {
                await roleManager.CreateAsync(new PortalRole("User"));
            }

            bool adminRoleExists = await roleManager.RoleExistsAsync("Admin");
            if (!adminRoleExists)
            {
                await roleManager.CreateAsync(new PortalRole("Admin"));
            }

            var superUserEmails = new[] {
                            "fluo@my.harrisburg.edu",
                            "AQureshi@harrisburgU.edu"
                        };

            foreach (var email in superUserEmails)
            {
                var exists = await dbContext.Users.AnyAsync(x => x.UserName == email, CancellationToken.None);

                if (!exists)
                {
                    var user = new PortalUser();
                    user.IsActive = true;
                    user.Email = email;
                    user.UserName = email;

                    var result = await userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        var userId = await userManager.GetUserIdAsync(user);
                        await userManager.AddToRoleAsync(user, "SuperUser");
                        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        await userManager.ConfirmEmailAsync(user, code);
                    }
                }
            }

            var adminEmails = new[] {
                            "fluo@my.harrisburg.edu",
                        };

            foreach (var email in adminEmails)
            {
                var exists = await dbContext.Users.AnyAsync(x => x.UserName == email, CancellationToken.None);

                if (!exists)
                {
                    var user = new PortalUser();
                    user.IsActive = true;
                    user.Email = email;
                    user.UserName = email;

                    var result = await userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        var userId = await userManager.GetUserIdAsync(user);
                        await userManager.AddToRoleAsync(user, "Admin");
                        await userManager.AddClaimAsync(user, new Claim("OrganizationId",
                            user.OrganizationId.ToString()));
                        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        await userManager.ConfirmEmailAsync(user, code);
                    }
                }
            }
        }

    }
}
