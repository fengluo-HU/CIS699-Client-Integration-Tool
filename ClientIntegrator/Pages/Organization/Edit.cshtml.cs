using ClientIntegrator.DataAccess;
using ClientIntegrator.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClientIntegrator.Pages.Organization
{
    [AuthorizeAttribute(Roles = "Admin, SuperUser")]
    public class EditOrganizationModel : PageModel
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        private readonly ILogger<EditOrganizationModel> logger;
        private readonly IConfiguration configuration;
        private readonly ClientIntegratorDbContext dbContext;
        public DataAccess.Models.Organization Organization { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public EditOrganizationModel(
            ILogger<EditOrganizationModel> logger,
            IConfiguration configuration,
            ClientIntegratorDbContext dbContext)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.dbContext = dbContext;
        }

        public class InputModel
        {
            [Display(Name = "Organization Id")]
            public long Id { get; set; }

            [Required(ErrorMessage = "Please provide an ClientId.")]
            [Display(Name = "Client Id")]
            public string ClientId { get; set; }

            [Required(ErrorMessage = "Please provide an ClientSecret.")]
            [Display(Name = "Client Secret")]
            public string ClientSecret { get; set; }

            [Required]
            [Display(Name = "Organization Name")]
            public string Name { get; set; }

        }
        public async Task<IActionResult> OnGetAsync(long? Id)
        {
            Organization = new DataAccess.Models.Organization();
            if (Id != null)
            {
                Organization = await dbContext.Organizations.Where(o => o.Id == Id).FirstOrDefaultAsync();
                if (Organization == null)
                {
                    Message = "There is no such Organization";
                    return NotFound();
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            DataAccess.Models.Organization organization = null;
            organization = Input.Id == 0 ? new DataAccess.Models.Organization() : await dbContext.Organizations.Where(o => o.Id == Input.Id).FirstOrDefaultAsync();
            if (ModelState.IsValid)
            {
                organization.DisplayName = Input.Name;
                organization.ClientId = Input.ClientId;
                organization.ClientSecret = Input.ClientSecret;
                if(Input.Id == 0) dbContext.Organizations.Add(organization);
                await dbContext.SaveChangesAsync();
                return LocalRedirect("/Organization/Index");
            }
            else
            {
                SetPageModel(organization);
            }
            
            return Page();
        }

        private void SetPageModel(DataAccess.Models.Organization organization)
        {
            Organization = organization;
            Input = new InputModel()
            {
                Id = organization.Id,
                Name = organization.DisplayName,
                ClientId = organization.ClientId,
                ClientSecret = organization.ClientSecret
            };
        }

    }
}
