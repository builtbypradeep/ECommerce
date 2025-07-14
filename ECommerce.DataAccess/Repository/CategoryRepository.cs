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
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;   
        }

       

        public void Update(Category obj)
        {
            //_db.Update(obj);
            _db.categories.Update(obj);
        }
    }
}
