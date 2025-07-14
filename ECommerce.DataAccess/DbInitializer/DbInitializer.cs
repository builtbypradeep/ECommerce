using ECommerce.DataAccess.Data;
using ECommerce.Models;
using ECommerce.utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager; 
            _roleManager = roleManager;
            _db = db;

        }
        public void Initialize()
        {
            //Add Migration if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }

            catch(Exception ex) { }

            //Create Role if they are not created
            if (!_roleManager.RoleExistsAsync(StaticDetails.Role_Cust).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole (StaticDetails.Role_Cust)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Employee)).GetAwaiter().GetResult ();
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Company)).GetAwaiter().GetResult();

                //If roles are not created , then we will create Admin as well
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@Ecommerce@gmail.com",
                    Email = "admin@Ecommerce@gmail.com",
                    Name = "Admin",
                    PhoneNumber = "9921456687",
                    Address = "Ambernath",
                    City = "Ambernath",
                    State = "Maharashtra",
                    PostalCode = "421501"

                }, "Admin123*").GetAwaiter().GetResult();

                ApplicationUser user = _db.applicationUsers.FirstOrDefault(x => x.Email == "admin@Ecommerce@gmail.com");
                _userManager.AddToRoleAsync(user, StaticDetails.Role_Admin).GetAwaiter().GetResult();
            }

            return;

        }
    }
}
