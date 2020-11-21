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
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace ClientIntegrator.Areas.Identity.Pages.Account.Manage.Users
{

    [AuthorizeAttribute(Roles = "Admin, SuperUser")]
    public class CreateUserModel : PageModel
    {
        public SelectList Organizations { get; set; }
        public SelectList UserRoles { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
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
            public bool CreateSuperUser { get; set; }

        }

        private readonly UserManager<PortalUser> _userManager;
        private readonly IUserStore<PortalUser> _userStore;
        private readonly IUserEmailStore<PortalUser> _emailStore;
        private readonly ILogger<CreateUserModel> _logger;
        private readonly ClientIntegratorDbContext dbContext;

        public CreateUserModel(
            UserManager<PortalUser> userManager,
            IUserStore<PortalUser> userStore,
            ILogger<CreateUserModel> logger,
            ClientIntegratorDbContext dbContext)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            this.dbContext = dbContext;
        }

        public async Task OnGetAsync(string role = null)
        {
            Input = new InputModel();

            if (User.IsAdmin())
            {
                Input.OrganizationId = User.GetOrganizationId();
            }

            if (role == "SuperUser")
            {
                Input.CreateSuperUser = true;
                Input.OrganizationId = long.MaxValue;
            }

            await SetPageModel();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (Input.CreateSuperUser)
                {
                    if (User.IsSuperUser())
                    {
                        return await CreateSuperUser();
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                //organization Admin is not allowed to create a SuperUser
                if (Input.Roles.Select(x => x.ToUpper()).Contains(Roles.SuperUser.ToString().ToUpper()))
                {
                    return Unauthorized();
                }

                using var scope = new TransactionScope(TransactionScopeOption.Required,
                   new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead },
                       TransactionScopeAsyncFlowOption.Enabled);
                var user = CreateUser();
                var portalUser = user as PortalUser;
                portalUser.IsActive = true;
                if (User.IsSuperUser())
                {
                    portalUser.OrganizationId = Input.OrganizationId;
                }
                else
                {
                    portalUser.OrganizationId = User.GetOrganizationId();
                }

                portalUser.UserName = Input.Email;
                portalUser.Email = Input.Email;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    foreach (var role in Input.Roles)
                    {
                        await _userManager.AddToRoleAsync(user, role);
                        _logger.LogInformation($"User {User.Identity.Name} created a new {role} account");
                    }

                    await _userManager.AddClaimAsync(user, new Claim("OrganizationId",
                            portalUser.OrganizationId.ToString()));

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _userManager.ConfirmEmailAsync(user, code);

                    scope.Complete();

                    _logger.LogInformation($"User {User.Identity.Name} created a new account with " +
                                            $"username {portalUser.UserName} for EhrOrganizationId {portalUser.OrganizationId}.");


                    return LocalRedirect("/Identity/Account/Users");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            await SetPageModel();

            return Page();
        }

        private PortalUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<PortalUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(PortalUser)}'. " +
                    $"Ensure that '{nameof(PortalUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<PortalUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<PortalUser>)_userStore;
        }

        private async Task<List<PortalRole>> GetRoles()
        {
            if (User.IsSuperUser())
            {
                return await dbContext.Roles.ToListAsync();
            }
            return await dbContext.Roles.Where(x => x.NormalizedName != "SUPERUSER").ToListAsync();
        }

        private async Task SetPageModel()
        {
            Organizations = new SelectList(await dbContext.Organizations.Select(x => x).ToListAsync(), nameof(Organization.Id), nameof(Organization.DisplayName));
            UserRoles = new SelectList(await GetRoles(), nameof(PortalRole.NormalizedName), nameof(PortalRole.Name));
        }

        private async Task<IActionResult> CreateSuperUser()
        {
            using var scope = new TransactionScope(TransactionScopeOption.Required,
                  new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead },
                      TransactionScopeAsyncFlowOption.Enabled);

            var user = CreateUser();
            var portalUser = user as PortalUser;
            portalUser.IsActive = true;
            portalUser.UserName = Input.Email;
            portalUser.Email = Input.Email;

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await _userManager.GetUserIdAsync(user);
                await _userManager.AddToRoleAsync(user, "SuperUser");
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _userManager.ConfirmEmailAsync(user, code);

                scope.Complete();

                return LocalRedirect("/Identity/Account/Manage/Users");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // If we got this far, something failed, redisplay form
            await SetPageModel();

            return Page();
        }
    }

}
