using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;

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
        public OAuth2Settings OAuth2 { get; set; }
    }
    public class OAuth2Settings
    {
        public string GrantType { get; set; } = "client_credentials";
        public string ClientId { get; set; }
        public string TenantId { get; set; }
        public string ClientSecret { get; set; }
        public string Scopes { get; set; } = "email,offline_access,https://outlook.office.com/SMTP.Send";//https://outlook.office.com/IMAP.AccessAsUser.All,https://outlook.office.com/POP.AccessAsUser.All,
        public string RedirectUrl { get; set; }//= $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/token";
    }
}
