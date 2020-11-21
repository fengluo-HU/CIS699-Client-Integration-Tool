using ClientIntegrator.Common.Extensions;
using ClientIntegrator.Common.Models;
using ClientIntegrator.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClientIntegrator.Areas.Identity.Pages.Account.Manage.Users
{
    [AuthorizeAttribute(Roles = "Admin, SuperUser")]
    public class IndexModel : PageModel
    {
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public string IdSort { get; set; }
        public string EmailSort { get; set; }

        public PaginatedList<UserModel> Users { get; set; }

        public class UserModel
        {
            public long Id { get; set; }
            public string Email { get; set; }
            public bool IsActive { get; set; }
            public long? OrganizationId { get; set; }
            public string OrganizationName { get; set; }
            public string Roles { get; set; }

        }

        private readonly ILogger<IndexModel> logger;
        private readonly IConfiguration configuration;
        private readonly ClientIntegratorDbContext dbContext;

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration configuration,
            ClientIntegratorDbContext dbContext)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.dbContext = dbContext;
        }

        public async Task OnGetAsync(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {

            CurrentSort = sortOrder;

            IdSort = sortOrder == "id" ? "id_desc" : "id";
            EmailSort = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<UserModel> users = dbContext.Users.Where(x => x.IsActive == true)
                 .Join(dbContext.UserRoles, x => x.Id, y => y.UserId, (x, y) => new { x.Id, x.Email, x.IsActive, x.OrganizationId, y.RoleId })
                 .Join(dbContext.Roles, x => x.RoleId, y => y.Id, (x, y) => new { x.Id, x.Email, x.IsActive, x.OrganizationId, RoleName = y.Name })
                 .GroupJoin(dbContext.Organizations, x => x.OrganizationId, y => y.Id, (x, y) => new { x.Id, x.Email, x.IsActive, x.OrganizationId, x.RoleName, Organization = y })
                 .SelectMany(x => x.Organization.DefaultIfEmpty(), (x, y) => new { x.Id, x.Email, x.IsActive, x.OrganizationId, x.RoleName, OrganizationName = y.DisplayName })
                 .AsEnumerable()
                 .GroupBy(x => x.Id)
                 .Select(x => new UserModel()
                 {
                     Id = x.Key,
                     Email = x.First().Email,
                     IsActive = x.First().IsActive,
                     OrganizationId = x.First().OrganizationId,
                     OrganizationName = x.First().OrganizationName,
                     Roles = string.Join(", ", x.Select(z => z.RoleName).OrderBy(z => z))

                 }).AsQueryable();
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.Email.ToLower().Contains(searchString.ToLower()));
            }

            if (User.IsAdmin())
            {
                users = users.Where(s => s.OrganizationId == User.GetOrganizationId());
            }

            switch (sortOrder)
            {
                case "email_desc":
                    users = users.OrderByDescending(s => s.Email);
                    break;

                case "id":
                    users = users.OrderBy(s => s.Id);
                    break;

                case "id_desc":
                    users = users.OrderByDescending(s => s.Id);
                    break;

                default:
                    users = users.OrderBy(s => s.Email);
                    break;
            }

            int pageSize = configuration.GetValue<int>("PageSize");
            Users = await PaginatedList<UserModel>.CreateAsync(
                users.AsNoTracking(), pageIndex ?? 1, pageSize);

        }
    }
}
