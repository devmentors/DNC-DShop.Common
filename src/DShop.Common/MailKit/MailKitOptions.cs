using System;
using System.Collections.Generic;
using System.Text;

namespace DShop.Common.MailKit
{
    public class MailKitOptions
    {
        public string SmtpHost { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
