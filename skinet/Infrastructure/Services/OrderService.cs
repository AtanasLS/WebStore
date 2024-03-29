using Core.Entities;
using Core.Entities.OrederAggregate;
using Core.Interfaces;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IBasketRepository _basketRepo;

        public OrderService(IUnitOfWork unitOfWork, IBasketRepository basketRepo)
         {
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
         }
        public async Task<Order> CreateOrderAsync(string bayerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
        {
            //get the basket 
            var basket =  await _basketRepo.GetBasketAsync(basketId);
            //get the items from the basket
            var items = new List<OrderItem>();
            foreach(var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name,
                 productItem.PictureUrl);
                 var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
                 items.Add(orderItem);
            }
            //get delivery method from repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            //calc subtotal
            var subtotal = items.Sum(item => item.Price * item.Quanitity);

            //create Order 
            var order = new Order(items, bayerEmail, shippingAddress, deliveryMethod, subtotal);
            
            _unitOfWork.Repository<Order>().Add(order);

            //save to db
            var result = await _unitOfWork.Complete();
             
            if(result <= 0) return null;

            // delete basket 
            await _basketRepo.DeleteBasketAsync(basketId);

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