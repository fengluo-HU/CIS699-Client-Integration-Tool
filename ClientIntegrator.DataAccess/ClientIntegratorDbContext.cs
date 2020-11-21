using ClientIntegrator.DataAccess.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Toolbelt.ComponentModel.DataAnnotations;

namespace ClientIntegrator.DataAccess
{

    public class ClientIntegratorDbContext : IdentityDbContext<PortalUser, PortalRole, long, PortalUserClaim, PortalUserRole, PortalUserLogin, PortalRoleClaim, PortalUserToken>,
        IDataProtectionKeyContext
    {
        public ClientIntegratorDbContext(DbContextOptions<ClientIntegratorDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Configuration> Configurations { get; set; }
        public virtual DbSet<ConfigurationDict> ConfigurationDicts { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.BuildIndexesFromAnnotations();
        }
    }
}
