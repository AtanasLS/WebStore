using Core.Entities.OrederAggregate;

namespace Core.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string bayerEmail, int deliveryMethod, string basketId, 
        Address shippingAddress);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string bayerEmail);
        Task<Order> GetOrderByIdAsync(int id, string bayerEmail);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
    }
}