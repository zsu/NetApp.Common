using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NetApp.Common
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private IEncryptionService _encryptionService;

        public EmailService(IOptions<EmailSettings> emailSettings):this(emailSettings?.Value)
        {}
        public EmailService(IOptions<EmailSettings> emailSettings, IEncryptionService encryption):this(emailSettings?.Value)
        {}
        public EmailService(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }
        public EmailService(EmailSettings emailSettings, IEncryptionService encryption)
        {
            _emailSettings = emailSettings;
            _encryptionService = encryption;
            if (_encryptionService != null)
            {
                if (!string.IsNullOrWhiteSpace(_emailSettings.Password))
                    _emailSettings.Password = _encryptionService.Decrypt(_emailSettings.Password);
                if (_emailSettings.OAuth2 != null && !string.IsNullOrWhiteSpace(_emailSettings.OAuth2.ClientSecret))
                    _emailSettings.OAuth2.ClientSecret = _encryptionService.Decrypt(_emailSettings.OAuth2.ClientSecret);
            }
        }
        ///// <summary>
        ///// send email with UTF-8
        ///// </summary>
        ///// <param name="mailTo">consignee email,multi split with ","</param>
        ///// <param name="subject">subject</param>
        ///// <param name="message">email message</param>
        ///// <param name="isHtml">is set message as html</param>
        //public void Send(string mailTo, string subject, string message, bool isHtml = false, List<string> attachments = null)
        //{
        //    SendEmail(mailTo, null, null, subject, message, Encoding.UTF8, isHtml, attachments);
        //}

        ///// <summary>
        ///// send email
        ///// </summary>
        ///// <param name="mailTo">consignee email,multi split with ","</param>
        ///// <param name="subject">subject</param>
        ///// <param name="message">email message</param>
        ///// <param name="encoding">email message encoding</param>
        ///// <param name="isHtml">is set message as html</param>
        //public void Send(string mailTo, string subject, string message, Encoding encoding, bool isHtml = false, List<string> attachments = null)
        //{
        //    SendEmail(mailTo, null, null, subject, message, encoding, isHtml, attachments);
        //}

        ///// <summary>
        ///// send email with UTF-8
        ///// </summary>
        ///// <param name="mailTo">consignee email,multi split with ","</param>
        ///// <param name="mailCc">send cc,multi split with ","</param>
        ///// <param name="mailBcc">send bcc,multi split with ","</param>
        ///// <param name="subject">subject</param>
        ///// <param name="message">email message</param>
        ///// <param name="isHtml">is set message as html</param>
        //public void Send(string mailTo, string mailCc, string mailBcc, string subject, string message, bool isHtml = false, List<string> attachments = null)
        //{
        //    SendEmail(mailTo, mailCc, mailBcc, subject, message, Encoding.UTF8, isHtml, attachments);
        //}

        ///// <summary>
        ///// send email
        ///// </summary>
        ///// <param name="mailTo">consignee email,multi split with ","</param>
        ///// <param name="mailCc">send cc,multi split with ","</param>
        ///// <param name="mailBcc">send bcc,multi split with ","</param>
        ///// <param name="subject">subject</param>
        ///// <param name="message">email message</param>
        ///// <param name="encoding">email message encoding</param>
        ///// <param name="isHtml">is set message as html</param>
        //public void Send(string mailTo, string mailCc, string mailBcc, string subject, string message, Encoding encoding, bool isHtml = false, List<string> attachments = null)
        //{
        //    SendEmail(mailTo, mailCc, mailBcc, subject, message, encoding, isHtml, attachments);
        //}
        //private void SendEmail(string mailTo, string mailCc, string mailBcc, string subject, string message, Encoding encoding, bool isHtml, List<string> attachments)
        //{
        //    SendEmailAsync(mailTo, mailCc, mailBcc, subject, message, encoding, isHtml, attachments).GetAwaiter().GetResult();
        //}

        /// <summary>
        /// send email with UTF-8 async
        /// </summary>
        /// <param name="mailTo">consignee email,multi split with ","</param>
        /// <param name="subject">subject</param>
        /// <param name="message">email message</param>
        /// <param name="isHtml">is set message as html</param>
        public Task SendAsync(string mailTo, string subject, string message, bool isHtml = false, List<string> attachments = null)
        {
            return SendEmailAsync(mailTo, null, null, subject, message, Encoding.UTF8, isHtml, attachments);
            //return Task.Factory.StartNew(() =>
            //{
            //    SendEmail(mailTo, null, null, subject, message, Encoding.UTF8, isHtml, attachments);
            //});
        }

        /// <summary>
        /// send email async
        /// </summary>
        /// <param name="mailTo">consignee email,multi split with ","</param>
        /// <param name="subject">subject</param>
        /// <param name="message">email message</param>
        /// <param name="encoding">email message encoding</param>
        /// <param name="isHtml">is set message as html</param>

        public Task SendAsync(string mailTo, string subject, string message, Encoding encoding, bool isHtml = false, List<string> attachments = null)
        {
            return SendEmailAsync(mailTo, null, null, subject, message, encoding, isHtml, attachments);
            //return Task.Factory.StartNew(() =>
            //{
            //    SendEmail(mailTo, null, null, subject, message, encoding, isHtml, attachments);
            //});
        }

        /// <summary>
        /// send email with UTF-8 async
        /// </summary>
        /// <param name="mailTo">consignee email,multi split with ","</param>
        /// <param name="mailCc">send cc,multi split with ","</param>
        /// <param name="mailBcc">send bcc,multi split with ","</param>
        /// <param name="subject">subject</param>
        /// <param name="message">email message</param>
        /// <param name="isHtml">is set message as html</param>
        public Task SendAsync(string mailTo, string mailCc, string mailBcc, string subject, string message, bool isHtml = false, List<string> attachments = null)
        {
            return SendEmailAsync(mailTo, mailCc, mailBcc, subject, message, Encoding.UTF8, isHtml, attachments);
            //return Task.Factory.StartNew(() =>
            //{
            //    SendEmail(mailTo, mailCc, mailBcc, subject, message, Encoding.UTF8, isHtml, attachments);
            //});
        }

        /// <summary>
        /// send email async
        /// </summary>
        /// <param name="mailTo">consignee email,multi split with ","</param>
        /// <param name="mailCc">send cc,multi split with ","</param>
        /// <param name="mailBcc">send bcc,multi split with ","</param>
        /// <param name="subject">subject</param>
        /// <param name="message">email message</param>
        /// <param name="encoding">email message encoding</param>
        /// <param name="isHtml">is set message as html</param>
        public Task SendAsync(string mailTo, string mailCc, string mailBcc, string subject, string message, Encoding encoding, bool isHtml = false, List<string> attachments = null)
        {
            return SendEmailAsync(mailTo, mailCc, mailBcc, subject, message, encoding, isHtml, attachments);
            //return Task.Factory.StartNew(() =>
            //{
            //    SendEmail(mailTo, mailCc, mailBcc, subject, message, encoding, isHtml, attachments);
            //});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailTo"></param>
        /// <param name="mailCc"></param>
        /// <param name="mailBcc"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="encoding"></param>
        /// <param name="isHtml"></param>
        private async Task SendEmailAsync(string mailTo, string mailCc, string mailBcc, string subject, string message, Encoding encoding, bool isHtml, List<string> attachments)
        {
            var _to = new string[0];
            var _cc = new string[0];
            var _bcc = new string[0];
            if (!string.IsNullOrEmpty(mailTo))
                _to = mailTo.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            if (!string.IsNullOrEmpty(mailCc))
                _cc = mailCc.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            if (!string.IsNullOrEmpty(mailBcc))
                _bcc = mailBcc.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            if (_to?.Length == 0 && _cc?.Length == 0 && _bcc?.Length == 0)
                throw new Exception("Receiver list cannot be empty.");
            var mimeMessage = new MimeMessage();
            var testEmail = _emailSettings.TestEmail;
            //add mail from
            mimeMessage.From.Add(new MailboxAddress(_emailSettings.Sender, _emailSettings.SenderEmail));
            if (testEmail == null)
            {
                //add mail to 
                foreach (var to in _to)
                {
                    mimeMessage.To.Add(MailboxAddress.Parse(to));
                }

                //add mail cc
                foreach (var cc in _cc)
                {
                    mimeMessage.Cc.Add(MailboxAddress.Parse(cc));
                }

                //add mail bcc 
                foreach (var bcc in _bcc)
                {
                    mimeMessage.Bcc.Add(MailboxAddress.Parse(bcc));
                }
            }
            else if (testEmail.Trim() == string.Empty)
                return;
            else
                mimeMessage.To.Add(MailboxAddress.Parse(testEmail));

            if ((mimeMessage.To == null || mimeMessage.To.Count == 0) && (mimeMessage.Cc == null || mimeMessage.Cc.Count == 0) && (mimeMessage.Bcc == null || mimeMessage.Bcc.Count == 0))
                return;
            //add subject
            mimeMessage.Subject = subject;
            var builder = new BodyBuilder();
            //add email body
            if (attachments != null)
                foreach (var file in attachments)
                    builder.Attachments.Add(file);
            TextPart body = null;

            if (isHtml)
            {
                body = new TextPart(TextFormat.Html);
            }
            else
            {
                body = new TextPart(TextFormat.Text);
            }
            //set email encoding
            body.SetText(encoding, message);
            if (isHtml)
            { builder.HtmlBody = body.Text; }
            else
            {
                builder.TextBody = body.Text;
            }
            //set email body
            mimeMessage.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync(_emailSettings.Smtp, _emailSettings.Port, !_emailSettings.UseSsl ? SecureSocketOptions.None : SecureSocketOptions.Auto).ConfigureAwait(false);
                if (_emailSettings.OAuth2 != null)
                {
                    var accessToken =  await GetAccessTokenAsync().ConfigureAwait(false);
                    await client.AuthenticateAsync(accessToken).ConfigureAwait(false);
                }
                else if (!string.IsNullOrWhiteSpace(_emailSettings.UserName))
                    await client.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password).ConfigureAwait(false);
                await client.SendAsync(mimeMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
        private async Task<SaslMechanismOAuth2> GetAccessTokenAsync()
        {
            var attributes = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", _emailSettings.OAuth2.GrantType),
                new KeyValuePair<string, string>("client_id", _emailSettings.OAuth2.ClientId),
                new KeyValuePair<string, string>("client_secret", _emailSettings.OAuth2.ClientSecret),
                new KeyValuePair<string, string>("scope", _emailSettings.OAuth2.Scopes),
            };
            var content = new FormUrlEncodedContent(attributes);
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(_emailSettings.OAuth2.TokenUri, content).ConfigureAwait(false);
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var json = JObject.Parse(responseString);
                var token = json["access_token"];
                return token != null
                    ? new SaslMechanismOAuth2(_emailSettings.SenderEmail, token.ToString())
                    : null;
            }
        }
        //private SaslMechanismOAuth2 GetAccessToken()
        //{
        //    try
        //    {
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //        using (var client = new WebClient())
        //        {
        //            var parameters = new NameValueCollection
        //    {
        //        {"grant_type", "client_credentials"},
        //        {"client_id", _emailSettings.OAuth2.ClientId},
        //        {"client_secret", _emailSettings.OAuth2.ClientSecret},
        //        {"scope", _emailSettings.OAuth2.Scopes}
        //    };

        //            byte[] response = client.UploadValues(_emailSettings.OAuth2.TokenUri, parameters);
        //            string result = System.Text.Encoding.UTF8.GetString(response);
        //            string token = JObject.Parse(result)["access_token"].ToString();
        //            return token != null
        //                    ? new SaslMechanismOAuth2(_emailSettings.SenderEmail, token)
        //                    : null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Failed to get access token: {ex.Message}", ex);
        //    }
        //}
    }
}
