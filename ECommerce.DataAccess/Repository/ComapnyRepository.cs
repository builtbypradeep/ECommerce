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
    public class ComapnyRepository : Repository<Companies>, IComapanyRepository
    {
        private readonly ApplicationDbContext _db;

        public  ComapnyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

      

        public void Update(Companies obj)
        {

            if (obj != null)
            {
                var comp = _db.companies.FirstOrDefault(x => x.Id == obj.Id);

                if (comp != null)
                {
                    comp.Id = obj.Id;
                    comp.Name = obj.Name;
                    comp.Address = obj.Address;
                    comp.City = obj.City;
                    comp.State = obj.State;
                    comp.PostalCode = obj.PostalCode;
                    comp.PhoneNumber = obj.PhoneNumber;

                    _db.companies.Update(comp);
                }
            }
        }
    }
}
