namespace OrderService.Models
{
    public class OrderItem
    {
        public Guid OrderItemId { get; set; }
        public int ProductId { get; set; }       
        public int Quantity { get; set; }       

       
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
    }
}
