namespace Sell_​_cleaning_services_e_commerce.Areas.MailService
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
