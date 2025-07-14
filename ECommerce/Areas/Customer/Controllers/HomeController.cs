using ECommerce.DataAccess.Repository;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models;
using ECommerce.utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace ECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;   
        }

        public IActionResult Index()
        {
            var ClaimsIndentity = (ClaimsIdentity)User.Identity;
            var claims = ClaimsIndentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claims != null)
            {
                HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                 (_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claims.Value).Count()));
            }

            IEnumerable<Product> productslist = _unitOfWork.Product.GetAll(includeproperties:"Category");
            return View(productslist);
        }


        public IActionResult Details(int id)
        {
            //Product productDet = _unitOfWork.Product.Get(u => u.Id == id, includeproperties:"Category");

            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == id, includeproperties: "Category"),
                Count = 1,
                ProductId = id
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            if (cart.Id > 0)
            {
                cart.Id = 0;
            }
           
           var claimsIdentity = (ClaimsIdentity)User.Identity;

            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            cart.ApplicationUserId = userId;

            ShoppingCart cartfromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId
            && u.ProductId == cart.ProductId);

            if (cartfromDb != null)
            {
                cartfromDb.Count += cart.Count;
                _unitOfWork.ShoppingCart.Update(cartfromDb);
                _unitOfWork.Save();
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(cart);
                
                _unitOfWork.Save();
               
            }

            TempData["success"] = "Cart updated successfully.";

            HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                  (_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count()));


            return RedirectToAction("Index");

            
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
