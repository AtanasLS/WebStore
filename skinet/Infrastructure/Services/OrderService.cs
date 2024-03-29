using Core.Entities;
using Core.Entities.OrederAggregate;
using Core.Interfaces;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        public readonly IGenericRepository<Order> _orderRepo;
        public readonly IGenericRepository<DeliveryMethod> _dmRepo;
        public readonly IGenericRepository<Product> _productRepo;
        public readonly IBasketRepository _basketRepo;

        public OrderService(IGenericRepository<Order> orderRepo, IGenericRepository<DeliveryMethod> dmRepo,
         IGenericRepository<Product> productRepo, IBasketRepository basketRepo)
         {
            _basketRepo = basketRepo;
            _productRepo = productRepo;
            _dmRepo = dmRepo;
            _orderRepo = orderRepo;

         }
        public async Task<Order> CreateOrderAsync(string bayerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
        {
            //get the basket 
            var basket =  await _basketRepo.GetBasketAsync(basketId);
            //get the items from the basket
            var items = new List<OrderItem>();
            foreach(var item in basket.Items)
            {
                var productItem = await _productRepo.GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name,
                 productItem.PictureUrl);
                 var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
                 items.Add(orderItem);
            }
            //get delivery method from repo
            var deliveryMethod = await _dmRepo.GetByIdAsync(deliveryMethodId);

            //calc subtotal
            var subtotal = items.Sum(item => item.Price * item.Quanitity);

            //create Order 
            var order = new Order(items, bayerEmail, shippingAddress, deliveryMethod, subtotal);

            //TODO: save to db

            //return the order
            return order;
        }

        public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetOrderByIdAsync(int id, string bayerEmail)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string bayerEmail)
        {
            throw new NotImplementedException();
        }
    }
}