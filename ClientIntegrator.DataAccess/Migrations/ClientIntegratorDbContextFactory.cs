using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClientIntegrator.DataAccess.Migrations
{
    public class ClientIntegratorDbContextFactory : IDesignTimeDbContextFactory<ClientIntegratorDbContext>
    {
        public ClientIntegratorDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ClientIntegratorDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Database=client_integrator_local;Username=postgres;Password=postgres-password",
                    x => x.MigrationsHistoryTable("migration_history", "admin"));

            return new ClientIntegratorDbContext(optionsBuilder.Options);
        }
    }
}
