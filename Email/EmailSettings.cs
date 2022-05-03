using System;
using System.Collections.Generic;
using System.Text;

namespace NetApp.Common
{
    public class EmailSettings
    {
        public string Smtp { get; set; }
        public int Port { get; set; }
        public string Sender { get; set; }
        public string SenderEmail { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
        public string TestEmail { get; set; }
    }
}
