using System.Net;
using System.Net.Mail;

namespace Sell_​_cleaning_services_e_commerce.Areas.MailService
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            //var mailServer = "mail@gmail.com";

            //var pw = "pw pw pw pw";

       

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mailServer, pw)
            };

            return client.SendMailAsync(
                new MailMessage(from: mailServer,
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
