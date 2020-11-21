using ClientIntegrator.DataAccess;
using ClientIntegrator.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientIntegrator.Common.Services
{
    public interface ILayoutService
    {
        Task<Organization> GetOrganizationByUserId(long userId);
        void SetSelectedOrganizationId(long userId, long orgId);
        void ReleaseCache(long userId);
        Task<List<Organization>> GetOrganizationsForSuperUser(long userId);
    }
    public class LayoutService : ILayoutService
    {
        private readonly ILogger<LayoutService> logger;
        private readonly ClientIntegratorDbContext dbContext;
        private static ConcurrentDictionary<long, long> selectedOrganizationCache =
            new ConcurrentDictionary<long, long>();

        public LayoutService(ClientIntegratorDbContext dbContext, ILogger<LayoutService> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }
        public async Task<Organization> GetOrganizationByUserId(long userId)
        {
            logger.LogInformation($"Inside LayoutService");
            if (selectedOrganizationCache.TryGetValue(userId, out long retrievedValue))
            {
                logger.LogInformation($"Get organization for userId {userId} with organization ID {retrievedValue}");
                return await dbContext.Organizations.SingleOrDefaultAsync(x => x.Id == retrievedValue);
            }
            var user = await dbContext.Users.Include(x => x.Organization).SingleOrDefaultAsync(x => x.Id == userId);
            logger.LogInformation($"Get organization for userId {userId} with organization ID {user?.Organization?.Id}");
            return user?.Organization;
        }

        public void SetSelectedOrganizationId(long userId, long orgId)
        {
            logger.LogInformation($"Inside LayoutService");
            logger.LogInformation($"User id {userId} currently selected organization ID {orgId}");
            if (selectedOrganizationCache.TryGetValue(userId, out long retrievedValue))
            {
                // Replace the old value with the new value.
                if (selectedOrganizationCache.TryUpdate(userId, orgId, retrievedValue))
                {
                    logger.LogInformation($"Change the selected organization ID from {retrievedValue} to {orgId}");
                }
            }
            else
            {
                if (selectedOrganizationCache.TryAdd(userId, orgId))
                {
                    logger.LogInformation($"Set selected organization ID to {orgId} for user ID {userId}");
                }
            }
        }

        public void ReleaseCache(long userId)
        {
            logger.LogInformation($"Inside LayoutService");
            selectedOrganizationCache.Clear();
            logger.LogInformation($"User id {userId} currently clear out the selectedOrganizationCache");
        }

        public async Task<List<Organization>> GetOrganizationsForSuperUser(long userId)
        {
            logger.LogInformation($"Inside LayoutService");
            logger.LogInformation($"Get list of organizations for userId {userId}");
            var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);
                
            if (user != null)
            {
                return await dbContext.Organizations.ToListAsync();
            }
            return null;
        }
    }
}
