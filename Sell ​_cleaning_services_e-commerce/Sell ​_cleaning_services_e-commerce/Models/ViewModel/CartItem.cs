using Newtonsoft.Json;

namespace Sell_​_cleaning_services_e_commerce.Models.ViewModel
{

    public class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; internal set; }
    }

    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
