using System.Linq.Expressions;
using Core.Entities.OrederAggregate;

namespace Core.Specifications
{
    public class OrdersWithItemsAndOrderingSpecifactions : BaseSpecification<Order>
    {
        public OrdersWithItemsAndOrderingSpecifactions(string email) : base(o => o.BayerEmail == 
        email)
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.DeliveryMethod);
            AddOrderByDescending(o => o.OrderDate);
        }

        public OrdersWithItemsAndOrderingSpecifactions(int id, string email) 
            : base(o => o.Id == id && o.BayerEmail == email)
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.DeliveryMethod);
        }
    }
}