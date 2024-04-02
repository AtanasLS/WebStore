


using Core.Entities.OrederAggregate;

namespace api.Dtos
{
    public class OrderToReturnDto
    {
         
        public int Id { get; set; }
        public string BayerEmail { get; set; }
        public DateTime OrderDate { get; set; } 
        public Address ShipToAddress { get; set; }
        public string DeliveryMethod { get; set; }
        public decimal ShippingPrice { get; set; }
        public IReadOnlyList<OrderItemDto> OrderItems { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } 


    }
}