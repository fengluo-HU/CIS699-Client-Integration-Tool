using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ClientIntegrator.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using ClientIntegrator.Common.Models;
using Microsoft.Extensions.Configuration;
using MailKit.Security;
using ClientIntegrator.Common.Services;
using System.Net.Mail;
using System.Net;
using System;
using Microsoft.Extensions.Logging;

namespace ClientIntegrator.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<PortalUser> _userManager;
        private readonly ISmtpEmailService _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ForgotPasswordModel> _logger;

        public ForgotPasswordModel(UserManager<PortalUser> userManager, ISmtpEmailService emailSender, 
            IConfiguration configuration, ILogger<ForgotPasswordModel> logger)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);


                var fromAddress = new MailAddress(_configuration["NoReplyEmailAddress"], "No Reply");
                var toAddress = new MailAddress(Input.Email, "To Name");
                var fromPassword = _configuration["NoReplyEmailPassword"];
                var subject = "Reset Password";
                var htmlMessage =
                    "Hi,<br/><br/>" +
                    "This email is being sent to you in response to resetting your password.<br/>" +
                    $"Please click <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a> to reset your password.<br/><br/>" +
                    "Thank You,<br/><br/>" +
                    " Client Integrator Team";

                try
                {
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                    };
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = htmlMessage,
                        IsBodyHtml = true
                    })
                    {
                        smtp.Send(message);
                    }
                }
                catch (Exception e)
                {

                    _logger.LogError(e, "An error occurred while sending the email request.");
                }

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
