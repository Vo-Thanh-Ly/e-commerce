using Sell__cleaning_services_e_commerce.Models.VnPayViewModel;

namespace Sell_​_cleaning_services_e_commerce.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
