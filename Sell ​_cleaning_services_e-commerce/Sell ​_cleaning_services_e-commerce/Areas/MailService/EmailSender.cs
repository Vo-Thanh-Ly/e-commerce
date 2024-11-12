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
                new MailMessage
                {
                    From = new MailAddress(mailServer),
                    To = { email },
                    Subject = subject,
                    Body = message,  // Nội dung email có thể là mã HTML
                    IsBodyHtml = true // Kích hoạt chế độ HTML
                });

        }
    }
}
