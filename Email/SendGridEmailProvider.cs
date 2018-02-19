using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Starship.Azure.Email {
    public class SendGridEmailProvider {
        
        public SendGridEmailProvider(string username, string password) {
            Client = new SmtpClient("smtp.sendgrid.net", 587);
            Client.Credentials = new NetworkCredential(username, password);
        }

        public void Send(string from, string to, string subject, string body) {
            var message = new MailMessage();
            
            message.To.Add(to);
            message.From = new MailAddress(from);
            message.Subject = subject;

            message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Plain));
            
            Client.Send(message);
        }

        private SmtpClient Client { get; set; }
    }
}