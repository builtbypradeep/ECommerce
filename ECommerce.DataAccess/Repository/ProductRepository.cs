using ECommerce.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Models;
using System.Linq.Expressions;
using ECommerce.DataAccess.Data;

namespace ECommerce.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;   
        }

       

        public void Update(Product obj)
        {
            //_db.Update(obj);
          //  _db.products.Update(obj);

            var objfromdb = _db.products.FirstOrDefault(u => u.Id == obj.Id);

            if (objfromdb != null)
            {
                objfromdb.Id = obj.Id;
                objfromdb.Title = obj.Title;
                objfromdb.ISBN = obj.ISBN;
                objfromdb.Description = obj.Description;
                objfromdb.Author = obj.Author;
                objfromdb.ListPrice = obj.ListPrice;
                objfromdb.Price = obj.Price;
                objfromdb.Price50 = obj.Price50;
                objfromdb.Price100 = obj.Price100;
                objfromdb.CategoryId = obj.CategoryId;

                if (obj.ImageUrl != null) 
                {
                    objfromdb.ImageUrl = obj.ImageUrl;
                }


            }
        }
    }
}
