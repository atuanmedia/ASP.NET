namespace CMS_Backend.Services
{
    public class OrderEmailItem
    {
        public string ProductName { get; set; } = "";
        public int    Quantity    { get; set; }
        public decimal UnitPrice  { get; set; }
    }

    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(
            string toEmail,
            string toName,
            int    orderId,
            string orderDate,
            string shippingAddress,
            string? notes,
            List<OrderEmailItem> items);
    }
}
