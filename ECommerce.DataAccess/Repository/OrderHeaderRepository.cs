using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;

        }

        public void Update(OrderHeader obj)
        {
            _db.orderHeaders.Update(obj);
        }

        public void UpdateStatus(int id, string OrderStatus, string? PaymentStatus = null)
        {
            var OrderFormDb = _db.orderHeaders.FirstOrDefault(x => x.Id == id);

            if (OrderFormDb != null)
            {
                OrderFormDb.OrderStatus = OrderStatus;

                if (!string.IsNullOrEmpty(PaymentStatus))
                {
                    OrderFormDb.PaymentStatus = PaymentStatus;
                }
            }
        }

        public void UpdateStripePaymentID(int id, string SessionId, string PaymentIntentId)
        {
            var OrderFromDb = _db.orderHeaders.FirstOrDefault(x => x.Id == id);

            if (OrderFromDb != null)
            {
                if (!string.IsNullOrEmpty(SessionId))
                {
                    OrderFromDb.SessionId = SessionId;
                }

                if (!string.IsNullOrEmpty(PaymentIntentId))
                {
                    OrderFromDb.PaymentIntentId = PaymentIntentId;
                    OrderFromDb.OrderDate = DateTime.Now;
                }
            }
           
            
        }
    }
}
