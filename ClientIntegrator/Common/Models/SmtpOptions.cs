using MailKit.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientIntegrator.Common.Models
{
    public class SmtpOptions
    {
        public string Server { get; set; } = string.Empty;
        public int Port { get; set; } = 25;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool UseSsl { get; set; } = false;
        public SecureSocketOptions SecureSocketOptions { get; set; } = SecureSocketOptions.Auto;
        public bool RequiresAuthentication { get; set; } = false;
        public string PreferredEncoding { get; set; } = string.Empty;
    }
}
