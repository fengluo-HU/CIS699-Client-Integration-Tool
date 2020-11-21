using ClientIntegrator.Common.Extensions;
using ClientIntegrator.Common.Services;
using ClientIntegrator.DataAccess;
using ClientIntegrator.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientIntegrator.Pages.Organization
{
    [AuthorizeAttribute(Roles = "Admin, SuperUser")]
    public class OrganizationModel : PageModel
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public List<DataAccess.Models.Organization> Organizations;

        private readonly ILogger<OrganizationModel> logger;
        private readonly IConfiguration configuration;
        private readonly ClientIntegratorDbContext dbContext;
        public OrganizationModel(
            ILogger<OrganizationModel> logger,
            IConfiguration configuration,
            ClientIntegratorDbContext dbContext)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.dbContext = dbContext;
        }

        public async Task OnGetAsync()
        {
            Organizations = await dbContext.Organizations.ToListAsync();
            if (Organizations == null)
            {
                Message = "There is no such List";
            }
        }
    }
}
