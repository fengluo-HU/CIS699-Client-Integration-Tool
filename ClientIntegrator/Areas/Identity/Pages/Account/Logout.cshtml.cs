using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ClientIntegrator.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ClientIntegrator.Common.Services;
using ClientIntegrator.Common.Extensions;

namespace ClientIntegrator.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<PortalUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly ILayoutService _layoutService;

        public LogoutModel(SignInManager<PortalUser> signInManager, ILogger<LogoutModel> logger, ILayoutService layoutService)
        {
            _signInManager = signInManager;
            _logger = logger;
            _layoutService = layoutService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            //  Clear out Cache
            var userId = User.GetLoggedInUserId<int>();
            _layoutService.ReleaseCache(userId);
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
