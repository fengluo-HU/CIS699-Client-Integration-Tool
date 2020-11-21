using System;
using System.Threading.Tasks;
using ClientIntegrator.Common.Models;
using MailKit.Net.Smtp;
using MimeKit;
using NLog;

namespace ClientIntegrator.Common.Services
{
    public interface ISmtpEmailService
    {
        Task SendEmailAsync(SmtpOptions smtpOptions,
            string to,
            string from,
            string subject,
            string plainTextMessage,
            string htmlMessage,
            string replyTo = null);
        Task SendSingleEmailAsync(SmtpOptions smtpOptions,
            string toCsv,
            string from,
            string subject,
            string plainTextMessage,
            string htmlMessage,
            string replyTo = null);
        Task SendMultipleEmailAsync(SmtpOptions smtpOptions,
            string toCsv,
            string from,
            string subject,
            string plainTextMessage,
            string htmlMessage);
    }
    public class SmtpEmailService : ISmtpEmailService
    {
        private static Logger _logger = LogManager.GetLogger("SmtpEmailService");
        /// <summary>
        /// Send email to single email id
        /// </summary>
        public async Task SendSingleEmailAsync(
            SmtpOptions smtpOptions,
            string to,
            string from,
            string subject,
            string plainTextMessage,
            string htmlMessage,
            string replyTo = null)
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException("no to address provided");
            }

            if (string.IsNullOrWhiteSpace(from))
            {
                throw new ArgumentException("no from address provided");
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("no subject provided");
            }

            var hasPlainText = !string.IsNullOrWhiteSpace(plainTextMessage);
            var hasHtml = !string.IsNullOrWhiteSpace(htmlMessage);
            if (!hasPlainText && !hasHtml)
            {
                throw new ArgumentException("no message provided");
            }

            var m = new MimeMessage();

            m.From.Add(new MailboxAddress("", from));
            if (!string.IsNullOrWhiteSpace(replyTo))
            {
                m.ReplyTo.Add(new MailboxAddress("", replyTo));
            }
            m.To.Add(new MailboxAddress("", to));
            m.Subject = subject;

            //m.Importance = MessageImportance.Normal;
            //Header h = new Header(HeaderId.Precedence, "Bulk");
            //m.Headers.Add()

            BodyBuilder bodyBuilder = new BodyBuilder();
            if (hasPlainText)
            {
                bodyBuilder.TextBody = plainTextMessage;
            }

            if (hasHtml)
            {
                bodyBuilder.HtmlBody = htmlMessage;
            }

            m.Body = bodyBuilder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(
                        smtpOptions.Server,
                        smtpOptions.Port,
                        smtpOptions.UseSsl)
                        .ConfigureAwait(false);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    if (smtpOptions.RequiresAuthentication)
                    {
                        await client.AuthenticateAsync(smtpOptions.User, smtpOptions.Password)
                            .ConfigureAwait(false);
                    }

                    await client.SendAsync(m).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }
                _logger.Trace("Send Mail Successfully to {to}.", to);
            }
            catch (Exception e)
            {
                _logger.Trace("Send Mail failed to {to}.", to);
                _logger.Error(e, "An error occurred while sending the email request.");
            }
        }
        /// <summary>
        /// Send email to multiple email ids
        /// </summary>
        public async Task SendMultipleEmailAsync(
            SmtpOptions smtpOptions,
            string toCsv,
            string from,
            string subject,
            string plainTextMessage,
            string htmlMessage)
        {
            if (string.IsNullOrWhiteSpace(toCsv))
            {
                throw new ArgumentException("no to addresses provided");
            }

            if (string.IsNullOrWhiteSpace(from))
            {
                throw new ArgumentException("no from address provided");
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("no subject provided");
            }

            var hasPlainText = !string.IsNullOrWhiteSpace(plainTextMessage);
            var hasHtml = !string.IsNullOrWhiteSpace(htmlMessage);
            if (!hasPlainText && !hasHtml)
            {
                throw new ArgumentException("no message provided");
            }

            var m = new MimeMessage();
            m.From.Add(new MailboxAddress("", from));
            string[] adrs = toCsv.Split(',');

            foreach (string item in adrs)
            {
                if (!string.IsNullOrEmpty(item)) { m.To.Add(new MailboxAddress("", item)); ; }
            }

            m.Subject = subject;
            m.Importance = MessageImportance.High;

            BodyBuilder bodyBuilder = new BodyBuilder();
            if (hasPlainText)
            {
                bodyBuilder.TextBody = plainTextMessage;
            }

            if (hasHtml)
            {
                bodyBuilder.HtmlBody = htmlMessage;
            }

            m.Body = bodyBuilder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(
                        smtpOptions.Server,
                        smtpOptions.Port,
                        smtpOptions.UseSsl).ConfigureAwait(false);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    if (smtpOptions.RequiresAuthentication)
                    {
                        await client.AuthenticateAsync(
                            smtpOptions.User,
                            smtpOptions.Password).ConfigureAwait(false);
                    }

                    await client.SendAsync(m).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }
                _logger.Trace("Send Mail Successfully to {toCsv}.", toCsv);
            }
            catch (Exception e)
            {
                _logger.Trace("Send Mail failed to {toCsv}.", toCsv);
                _logger.Error(e, "An error occurred while sending the email request.");
            }
        }

        public async Task SendEmailAsync(SmtpOptions smtpOptions, string to, string from, string subject, string plainTextMessage, string htmlMessage, string replyTo = null)
        {
            //determine if we need send to multiple recipients
            if (to.ToLower().Contains(','))
            {
                await SendMultipleEmailAsync(smtpOptions, to, from, subject, plainTextMessage, htmlMessage);
            }
            else
            {
                await SendSingleEmailAsync(smtpOptions, to, from, subject, plainTextMessage, htmlMessage, replyTo);
            }
        }
    }
}

