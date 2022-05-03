using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetApp.Common
{
    public interface IEmailService
    {
        //void Send(string mailTo, string subject, string message, bool isHtml = false, List<string> attachments = null);
        //void Send(string mailTo, string subject, string message, Encoding encoding, bool isHtml = false, List<string> attachments = null);
        //void Send(string mailTo, string mailCc, string mailBcc, string subject, string message, bool isHtml = false, List<string> attachments = null);
        //void Send(string mailTo, string mailCc, string mailBcc, string subject, string message, Encoding encoding, bool isHtml = false, List<string> attachments = null);
        Task SendAsync(string mailTo, string subject, string message, bool isHtml = false, List<string> attachments = null);
        Task SendAsync(string mailTo, string subject, string message, Encoding encoding, bool isHtml = false, List<string> attachments = null);
        Task SendAsync(string mailTo, string mailCc, string mailBcc, string subject, string message, bool isHtml = false, List<string> attachments = null);
        Task SendAsync(string mailTo, string mailCc, string mailBcc, string subject, string message, Encoding encoding, bool isHtml = false, List<string> attachments = null);
    }
}