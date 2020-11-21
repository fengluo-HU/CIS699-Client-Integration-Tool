using ClientIntegrator.Common.Extensions;
using ClientIntegrator.Common.Models;
using ClientIntegrator.DataAccess;
using ClientIntegrator.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;

namespace ClientIntegrator.Areas.Identity.Pages.Account.Manage.Users
{

    [AuthorizeAttribute(Roles = "Admin, SuperUser")]
    public class EditUserModel : PageModel
    {
        public SelectList Organizations { get; set; }
        public SelectList UserRoles { get; set; }
        [Display(Name = "Email")]
        public string UserName { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public long Id { get; set; }

            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Range(1, long.MaxValue, ErrorMessage = "Please select an organization.")]
            [Display(Name = "Organization")]
            public long OrganizationId { get; set; }

            [Display(Name = "Optional Abilities")]
            public List<string> Roles { get; set; }

            [Required]
            [Display(Name = "Is Active")]
            public bool IsActive { get; set; }
        }


        private readonly UserManager<PortalUser> _userManager;
        private readonly ILogger<EditUserModel> _logger;
        private readonly ClientIntegratorDbContext dbContext;
        public EditUserModel(
            UserManager<PortalUser> userManager,
            ILogger<EditUserModel> logger,
            ClientIntegratorDbContext dbContext)
        {
            _userManager = userManager;
            _logger = logger;
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            UserName = user.UserName;
            var roles = await _userManager.GetRolesAsync(user as PortalUser);

            if (!User.HasPermission(user))
            {
                return Unauthorized();
            }

            await SetPageModel(user, roles);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await GetUser(id) as PortalUser;
            var roles = await _userManager.GetRolesAsync(user);
            try
            {
                if (user == null)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required,
                       new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead },
                           TransactionScopeAsyncFlowOption.Enabled))
                    {
                        var portalUser = user as PortalUser;

                        if (!User.HasPermission(portalUser))
                        {
                            return Unauthorized();
                        }

                        if (User.IsSuperUser())
                        {
                            if (portalUser.OrganizationId != Input.OrganizationId)
                            {
                                _logger.LogInformation($"Admin user {User.Identity.Name} updated EhrOrganizationId for " +
                                           $"username {portalUser.UserName} from {portalUser.OrganizationId} to {Input.OrganizationId}.");
                            }

                            portalUser.OrganizationId = Input.OrganizationId;
                        }
                        else
                        {
                            if (portalUser.OrganizationId != User.GetOrganizationId())
                            {
                                _logger.LogInformation($"Admin user {User.Identity.Name} updated OrganizationId for " +
                                 $"username {portalUser.UserName} from {portalUser.OrganizationId} to {User.GetOrganizationId()}.");
                            }

                            portalUser.OrganizationId = User.GetOrganizationId();
                        }

                        if (portalUser.IsActive != Input.IsActive)
                        {
                            _logger.LogInformation($"Admin user {User.Identity.Name} updated IsActive flag for " +
                                            $"username {portalUser.UserName} from {portalUser.IsActive} to {Input.IsActive}.");

                            portalUser.IsActive = Input.IsActive;
                        }

                        await dbContext.SaveChangesAsync();

                        //clear existing roles
                        await _userManager.RemoveFromRolesAsync(user, roles.ToList());
                        await _userManager.AddToRolesAsync(user, Input.Roles);
                        _logger.LogInformation($"Admin user {User.Identity.Name} updated Role for " +
                                               $"username {portalUser.UserName} from {string.Join(",", roles)} to {string.Join(",", Input.Roles)}.");
                        //clear existing claims
                        var claims = await _userManager.GetClaimsAsync(user);

                        if (!claims.Any(x => x.Type == "OrganizationId" && x.Value == portalUser.OrganizationId.ToString()))
                        {
                            await _userManager.RemoveClaimsAsync(user, claims);

                            var claim = new Claim("OrganizationId",
                                        portalUser.OrganizationId.ToString());

                            await _userManager.AddClaimAsync(user, claim);

                            _logger.LogInformation($"Admin user {User.Identity.Name} updated Claims for " +
                                                  $"username {portalUser.UserName} from {string.Join(",", claims.Select(x => $"{x.Type} : {x.Value}"))}" +
                                                  $" to { $"{claim.Type} : {claim.Value}"}.");
                        }

                        if (Input.Password != null)
                        {
                            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                            var result = await _userManager.ResetPasswordAsync(user, token, Input.Password);

                            if (result.Succeeded)
                            {
                                scope.Complete();

                                _logger.LogInformation($"Admin user {User.Identity.Name} updated password for " +
                                                        $"username {portalUser.UserName}.");

                                return LocalRedirect("/Identity/Account/Users");
                            }

                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                        else
                        {
                            scope.Complete();
                            return LocalRedirect("/Identity/Account/Users");
                        }

                    }
                }
                // If we got this far, something failed, redisplay form
                await SetPageModel(user as PortalUser, roles);
                return Page();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Edit user Failed!");
                // If we got this far, something failed, redisplay form
                await SetPageModel(user as PortalUser, roles);
                return Page();
            }
        }

        private async Task<PortalUser> GetUser(long? id)
        {
            return await dbContext.Users.FindAsync(id);
        }

        private async Task<List<PortalRole>> GetRoles()
        {
            if (User.IsSuperUser())
            {
                return await dbContext.Roles.ToListAsync();
            }
            return await dbContext.Roles.Where(x => x.NormalizedName != "SUPERUSER").ToListAsync();
        }
        private async Task SetPageModel(PortalUser user, IList<string> roles)
        {
            if (user.OrganizationId != null)
            {
                Input = new InputModel()
                {
                    Id = user.Id,
                    Roles = roles.ToList(),
                    OrganizationId = user.OrganizationId.Value,
                    IsActive = user.IsActive
                };
                UserRoles = new SelectList(await GetRoles(), nameof(PortalRole.NormalizedName),
                    nameof(PortalRole.Name));
            }
            else
            {
                Input = new InputModel()
                {
                    Id = user.Id,
                    Roles = roles.ToList(),
                    OrganizationId = User.GetOrganizationId(),
                    IsActive = user.IsActive
                };
                UserRoles = new SelectList(await GetRoles(), nameof(PortalRole.NormalizedName),
                    nameof(PortalRole.Name));
            }
            Organizations = new SelectList(await dbContext.Organizations.Select(x => x).ToListAsync(), nameof(Organization.Id), nameof(Organization.DisplayName));
        }
    }
}
