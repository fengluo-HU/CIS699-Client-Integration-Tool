using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
using ClientIntegrator.Common.Models;
using ClientIntegrator.DataAccess;
using ClientIntegrator.Common.Extensions;
using ClientIntegrator.Common.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClientIntegrator.Pages.Configuration
{
    public class IndexModel : PageModel
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public PaginatedList<ConfigurationVM> Configurations { get; set; }
        public string OrganizationName { get; set; }
        [BindProperty(SupportsGet = true)]
        public long SelectedOrgId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; } = "Id";
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        public string CurrentOrder { get; set; }

        // External Services
        private readonly ILogger<IndexModel> logger;
        private readonly IConfiguration configuration;
        private readonly ClientIntegratorDbContext dbContext;
        private readonly ILayoutService layoutService;
        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration configuration,
            ClientIntegratorDbContext dbContext,
            ILayoutService layoutService)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.dbContext = dbContext;
            this.layoutService = layoutService;
        }
        public async Task OnGetAsync()
        {
            CurrentOrder = SortOrder;
            SortOrder = SortOrder == "desc" ? "asc" : "desc";

            var userId = User.GetLoggedInUserId<int>();
            var organization = await layoutService.GetOrganizationByUserId(userId);
            if (organization != null)
            {
                SelectedOrgId = organization.Id;
            }

            await SetPageModel(SortOrder, SortColumn, PageIndex, SelectedOrgId);
        }

        public void OnPostSetSelectedOrganization(long selectedOrgId)
        {
            var userId = User.GetLoggedInUserId<int>();
            layoutService.SetSelectedOrganizationId(userId, selectedOrgId);
        }

        private async Task SetPageModel(string sortOrder, string sortColumn, int pageIndex, long orgId)
        {
            try
            {

                // Show all configurations for SuperUser
                var configurations =
                    orgId == 0 ? (from o in dbContext.Organizations
                                  from u in dbContext.Users
                                  from cd in dbContext.ConfigurationDicts
                                  join c in dbContext.Configurations
                                   on new { orgId = o.Id, userId = u.Id, cdId = cd.Id} equals new { orgId = c.OrganizationId, userId = c.CreateBy, cdId = c.ConfigurationDictId } into details
                                  from d in details
                                  select new ConfigurationVM()
                                  {
                                      Id = d.Id,
                                      OrganizationId = o.Id,
                                      CreateAt = d.CreateAt.ToString("yyyy-MM-dd HH:mm tt"),
                                      CreateBy = u.UserName,
                                      OrganizationName = o.DisplayName,
                                      Key = cd.Key,
                                      Value = d.Value,
                                      ConfigurationDictId = d.ConfigurationDictId,
                                      Notes = d.Notes,
                                      CreateDate = d.CreateAt // store it for sorting
                                  }) :
                                (from o in dbContext.Organizations
                                    from u in dbContext.Users
                                    from cd in dbContext.ConfigurationDicts
                                    join c in dbContext.Configurations
                                        on new { orgId = o.Id, userId = u.Id, cdId = cd.Id } equals new { orgId = c.OrganizationId, userId = c.CreateBy, cdId = c.ConfigurationDictId } into details
                                    from d in details
                                    where d.OrganizationId == orgId
                                    select new ConfigurationVM()
                                    {
                                        Id = d.Id,
                                        OrganizationId = o.Id,
                                        CreateAt = d.CreateAt.ToString("yyyy-MM-dd HH:mm tt"),
                                        CreateBy = u.UserName,
                                        OrganizationName = o.DisplayName,
                                        Key = cd.Key,
                                        Value = d.Value,
                                        ConfigurationDictId = d.ConfigurationDictId,
                                        Notes = d.Notes,
                                        CreateDate = d.CreateAt // store it for sorting
                                    });

                await SetSortedResults(configurations, sortOrder, sortColumn, pageIndex);
            }
            catch (Exception e)
            {
                logger.LogError(e, "SetPageModel failed");
                Message = "Loading Page failed!";
                Success = false;
            }

        }

        private async Task SetSortedResults(IQueryable<ConfigurationVM> configurations, string sortOrder, string sortColumn, int pageIndex)
        {
            if (!string.IsNullOrEmpty(sortColumn))
            {
                sortOrder ??= "desc";
                configurations = configurations.OrderBy($"{sortColumn} {sortOrder}");
            }

            int pageSize = configuration.GetValue<int>("PageSize");
            Configurations = await PaginatedList<ConfigurationVM>.CreateAsync(
                configurations.AsNoTracking(), pageIndex, pageSize);
        }
    }

    public class ConfigurationVM : BaseModel
    {
        public string OrganizationName { get; set; }
        public long OrganizationId { get; set; }
        public string Notes { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public long ConfigurationDictId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}