namespace Hotels.Web.Services
{
    //kullanıcı için anlaşılır mesaj
    public class BookingApiException:Exception
    {
        public BookingApiException(string message, Exception? inner = null) : base(message, inner) { }
    }
}
