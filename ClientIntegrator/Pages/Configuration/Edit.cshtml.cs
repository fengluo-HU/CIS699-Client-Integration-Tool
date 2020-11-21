using ClientIntegrator.Common.Extensions;
using ClientIntegrator.Common.Services;
using ClientIntegrator.DataAccess;
using ClientIntegrator.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClientIntegrator.Pages.Configuration
{
    public class EditConfigurationModel : PageModel
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public SelectList Organizations { get; set; }
        private readonly ILogger<EditConfigurationModel> logger;
        private readonly IConfiguration configuration;
        private readonly ClientIntegratorDbContext dbContext;
        private readonly ILayoutService layoutService;
        public ConfigurationVM Configuration { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public EditConfigurationModel(
            ILogger<EditConfigurationModel> logger,
            IConfiguration configuration,
            ClientIntegratorDbContext dbContext,
            ILayoutService layoutService)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.dbContext = dbContext;
            this.layoutService = layoutService;
        }

        public class InputModel
        {
            [Display(Name = "Configuration Id")]
            public long Id { get; set; }

            [Display(Name = "Configuration Notes")]
            public string Notes { get; set; }
            [Required]
            [Display(Name = "Configuration Key")]
            public string Key { get; set; }
            [Required]
            [Display(Name = "Configuration Value")]
            public string Value { get; set; }
            [Required]
            [Display(Name = "ConfigurationDict ID")]
            public long ConfigurationDictId { get; set; }

            [Required]
            [Display(Name = "Organization ID")]
            public long OrganizationId { get; set; }

        }
        public async Task OnGetAsync(long? Id)
        {
            var configuration = new DataAccess.Models.Configuration();
            if (Id != null)
            {
                configuration = await dbContext.Configurations.Where(o => o.Id == Id).Include(x => x.ConfigurationDict).FirstOrDefaultAsync();
                if (configuration == null)
                {
                    Message = "There is no such configuration";
                }
                await SetPageModel(configuration);
            }
            else
            {
                Configuration = new ConfigurationVM();
                var userId = User.GetLoggedInUserId<int>();
                var organization = await layoutService.GetOrganizationByUserId(userId);
                if (organization != null)
                {
                    Input = new InputModel()
                    {
                        OrganizationId = organization.Id
                    };
                }
                Organizations = new SelectList(await dbContext.Organizations.Select(x => x).ToListAsync(), nameof(DataAccess.Models.Organization.Id), nameof(DataAccess.Models.Organization.DisplayName));
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            DataAccess.Models.Configuration configuration = null;
            var userId = User.GetLoggedInUserId<int>();
            var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);
            configuration = Input.Id == 0 ? new DataAccess.Models.Configuration() : await dbContext.Configurations.Where(o => o.Id == Input.Id).Include(x => x.ConfigurationDict).FirstOrDefaultAsync();        
            if (ModelState.IsValid)
            {
                configuration.Value = Input.Value;
                configuration.Notes = Input.Notes;
                if (Input.Id == 0)
                {
                    configuration.CreateAt = DateTime.UtcNow;
                    configuration.CreateBy = userId;
                    configuration.OrganizationId = user.OrganizationId ?? Input.OrganizationId;

                    if (Input.ConfigurationDictId == 0)
                    {
                        configuration.ConfigurationDict = new ConfigurationDict()
                        {
                            CreateAt = DateTime.UtcNow,
                            CreateBy = userId,
                            Notes = Input.Notes,
                            IsActive = true,
                            IsEncrypted = false,
                            Key = Input.Key,
                            DefaultValue = Input.Value
                        };
                    }
                    dbContext.Configurations.Add(configuration);
                }
                else
                {
                    configuration.UpdateAt = DateTime.UtcNow;
                    configuration.UpdateBy = userId;
                    configuration.ConfigurationDict.Key = Input.Key;
                    configuration.ConfigurationDict.UpdateAt = DateTime.UtcNow;
                    configuration.ConfigurationDict.UpdateBy = userId;
                }

                await dbContext.SaveChangesAsync();
                return LocalRedirect("/Configuration/Index");
            }
            else
            {
                await SetPageModel(configuration);
            }
            
            return Page();
        }

        private async Task SetPageModel(DataAccess.Models.Configuration configuration)
        {
            Organizations = new SelectList(await dbContext.Organizations.Select(x => x).ToListAsync(), nameof(DataAccess.Models.Organization.Id), nameof(DataAccess.Models.Organization.DisplayName));

            Configuration = new ConfigurationVM()
            {
                Id = configuration.Id,
                Key = configuration.ConfigurationDict.Key,
                Value = configuration.Value,
                Notes = configuration.Notes,
                ConfigurationDictId = configuration.ConfigurationDictId,
                OrganizationId = configuration.OrganizationId
            };

            Input = new InputModel()
            {
                Id = configuration.Id,
                Key = configuration.ConfigurationDict.Key,
                Value = configuration.Value,
                Notes = configuration.Notes,
                ConfigurationDictId = configuration.ConfigurationDictId,
                OrganizationId = configuration.OrganizationId
            };
        }

    }
}
