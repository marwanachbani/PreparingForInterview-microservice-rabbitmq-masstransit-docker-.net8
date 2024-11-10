namespace OrderService.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

