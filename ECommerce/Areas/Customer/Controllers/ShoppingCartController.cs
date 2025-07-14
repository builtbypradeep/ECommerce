using ECommerce.DataAccess.Repository;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models;
using ECommerce.Models.ViewModels;
using ECommerce.utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace ECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController : Controller
    {

        public readonly IUnitOfWork _unitOfWork;
        public IConfiguration _iconfig;
        // public IEnumerable<ShoppingCart> shoppingCarts { get; set; }

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        //public ShoppingCart 
        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId,
                includeproperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                Product getProductDetailsonProductId = _unitOfWork.Product.Get(u => u.Id == cart.ProductId);
                //Product GetProductModel = _unitOfWork.Product.GetAll(u => u.Id == cart.ProductId);
                cart.Product = getProductDetailsonProductId;
                cart.Price = GetCartPrice(cart, getProductDetailsonProductId);

                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }

        private double GetCartPrice(ShoppingCart obj, Product Product)
        {
            // if (obj == null) return 0;

            if (obj.Count <= 50)
                return Product.Price;

            else if (obj.Count <= 100)
                return Product.Price50;

            else
                return Product.Price100;
        }

        public IActionResult Plus(int cartId)
        {
            var GetCartDetailsfromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            GetCartDetailsfromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(GetCartDetailsfromDb);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var GetCartDetailsfromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked:true);

            if (GetCartDetailsfromDb.Count <= 1)
            {
                //at that time remove product from db
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, 
                    _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == GetCartDetailsfromDb.ApplicationUserId).Count() - 1);
                _unitOfWork.ShoppingCart.Remove(GetCartDetailsfromDb);
            }
            else
            {
                GetCartDetailsfromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(GetCartDetailsfromDb);
            }

            _unitOfWork.Save();
           // HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == GetCartDetailsfromDb.ApplicationUserId).Count());
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var GetCartDetailsFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked:true);
            HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == GetCartDetailsFromDb.ApplicationUserId).Count() - 1);
            _unitOfWork.ShoppingCart.Remove(GetCartDetailsFromDb);
            _unitOfWork.Save();

            

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId, includeproperties: "Product"),
                OrderHeader = new(),

            };


            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == cart.ApplicationUserId);
                cart.ApplicationUser = applicationUser;

                ShoppingCartVM.OrderHeader.Name = cart.ApplicationUser.Name;
                ShoppingCartVM.OrderHeader.StreetAddress = cart.ApplicationUser.Address;
                ShoppingCartVM.OrderHeader.PhoneNumber = cart.ApplicationUser.PhoneNumber;
                ShoppingCartVM.OrderHeader.City = cart.ApplicationUser.City;
                ShoppingCartVM.OrderHeader.State = cart.ApplicationUser.State;
                ShoppingCartVM.OrderHeader.PostalCode = cart.ApplicationUser.PostalCode;

                Product product = _unitOfWork.Product.Get(u => u.Id == cart.ProductId);
                cart.Product = product;
                cart.Price = GetCartPrice(cart, product);

                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeproperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;

            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(i => i.Id == userId);



            foreach (var item in ShoppingCartVM.ShoppingCartList)
            {
                Product product = _unitOfWork.Product.Get(x => x.Id == item.ProductId);

                item.Product = product;
                item.Price = GetCartPrice(item, product);
                ShoppingCartVM.OrderHeader.OrderTotal += (item.Price * item.Count);

            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //For Customer
                ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.OrderStatusPending;
            }
            else
            {
                //For Company
                ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayement;
                ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.OrderStatusApproved;
            }

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();


            foreach (var item in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = item.Price,
                    Count = item.Count
                };

                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }


            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //Write Stripe Payment Method for Regular Customer



                //try
                //{
                //    var options = new Stripe.Checkout.SessionCreateOptions
                //    {
                //        //SuccessUrl = "https://example.com/success",

                //        SuccessUrl = domain + $"customer/shoppingcart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                //        CancelUrl = domain + "/customer/shoppingcart/Index",
                //        //StripeConfiguration.ApiKey = "sk_test_51RhD7qQxtiLhCXLQ9ipJM8f36SIaV87dyHxQwRTrV77UsXQocJ9KqLSVcmhWDPaw4VALAKAL6WGnVbSfPvGMcrcp00a4yNPErC"
                //        LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                //        //{
                //        //   new Stripe.Checkout.SessionLineItemOptions
                //        //   {
                //        //       Price = "price_1MotwRLkdIwHu7ixYcPLm5uZ",
                //        //       Quantity = 2,
                //        //   },
                //        //},
                //        Mode = "payment",


                //    };

                //    foreach (var item in ShoppingCartVM.ShoppingCartList)
                //    {
                //        var SessionLineItem = new SessionLineItemOptions
                //        {
                //            PriceData = new SessionLineItemPriceDataOptions
                //            {
                //                UnitAmount = (long)(item.Price * 100), //If Price is 10.50 it will create as 1050
                //                Currency = "usd",
                //                ProductData = new SessionLineItemPriceDataProductDataOptions
                //                {
                //                    Name = item.Product.Title
                //                }
                //            },
                //            Quantity = item.Count

                //        };
                //        options.LineItems.Add(SessionLineItem);
                //    }
                //    //var service = new Stripe.Checkout.SessionService();
                //    //Stripe.Checkout.Session session = service.Create(options);

                //    var service = new SessionService();

                //    Session session = service.Create(options);


                //    _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                //    _unitOfWork.Save();

                //    Response.Headers.Add("Location", session.Url);
                //    return new StatusCodeResult(303);

                //}
                //catch (Exception ex) { }

                var domain = "https://localhost:7256/";
                var Currency = "usd";
                var SuccessUrl = domain + $"customer/shoppingcart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}";
                var CancelUrl = domain + "/customer/shoppingcart/Index";

                //var options = new List<SessionListOptions>
                //{
                //    //PaymentMethodTypes = new List<string>
                //    //{
                //    //    "card"
                //    //},

                //    LineItems = new List<SessionListOptions>
                //    {

                //    }
                //};

                //var options = new SessionCreateOptions
                //{
                //    PaymentMethodTypes = new List<string>
                //    {
                //        "card"
                //    },

                //    LineItems = new List<SessionLineItemOptions>
                //    {
                //        new SessionLineItemOptions
                //        {
                //            PriceData = new SessionLineItemPriceDataOptions
                //            {
                //                Currency = Currency,
                //                UnitAmount = Convert.ToInt32(50) * 100,

                //                ProductData = new SessionLineItemPriceDataProductDataOptions
                //                {
                //                    Name = "Product_Name",
                //                    Description = "Product_Description"
                //                }
                //            },
                //            Quantity = 1
                //        }
                //    },

                //    Mode = "payment",
                //    SuccessUrl = SuccessUrl,
                //    CancelUrl = CancelUrl  
                //};

                //var service = new SessionService();
                //var session = service.Create(options);

                //return Redirect(session.Url);
            }

            //return View(ShoppingCartVM);

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {

            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == id, includeproperties: "ApplicationUser");

            if (orderHeader != null)
            {

                if (orderHeader.PaymentStatus != StaticDetails.PaymentStatusDelayedPayement)
                {
                    var service = new SessionService();
                    //Session session = service.Get(orderHeader.SessionId);

                    //if (session.PaymentStatus.ToLower() == "paid")
                    //{
                    //    _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                    //    _unitOfWork.OrderHeader.UpdateStatus(id, StaticDetails.OrderStatusApproved, StaticDetails.PaymentStatusApproved);
                    //    _unitOfWork.Save();
                    //}
                }

                List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.
                    GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

                _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
                _unitOfWork.Save();
            }
            return View(id);
        }
    }

}
