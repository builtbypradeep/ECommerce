using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models;
using ECommerce.Models.ViewModels;
using ECommerce.utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using System.Security.Claims;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //OrderVM = _unitOfWork.OrderHeader.GetAll();
            return View();
        }


        public IActionResult Delete(int orderId)
        {
            OrderVM = new()
            {
                orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeproperties: "ApplicationUser"),

                orderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeproperties: "Product")
            };

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == OrderVM.orderHeader.ApplicationUserId);
            OrderVM.orderHeader.ApplicationUser = applicationUser;

            foreach (var item in OrderVM.orderDetails)
            {
                Product product = _unitOfWork.Product.Get(x => x.Id == item.ProductId);
                item.Product = product;
            }


            return View(OrderVM);
        }


        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult UpdateOrderDetails()
        {

            var OrderDataFromDb = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVM.orderHeader.Id);

            OrderDataFromDb.Name = OrderVM.orderHeader.Name;
            OrderDataFromDb.PhoneNumber = OrderVM.orderHeader.PhoneNumber;
            OrderDataFromDb.StreetAddress = OrderVM.orderHeader.StreetAddress;
            OrderDataFromDb.City = OrderVM.orderHeader.City;
            OrderDataFromDb.State = OrderVM.orderHeader.State;
            OrderDataFromDb.PostalCode = OrderVM.orderHeader.PostalCode;
            //OrderDataFromDb.Name = OrderVM.orderHeader.Name;

            if (!string.IsNullOrEmpty(OrderVM.orderHeader.Carrier))
            {
                OrderDataFromDb.Carrier = OrderVM.orderHeader.Carrier;
            }

            if (!string.IsNullOrEmpty(OrderVM.orderHeader.TrackingNumber))
            {
                OrderDataFromDb.TrackingNumber = OrderVM.orderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(OrderDataFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully.";


            return RedirectToAction(nameof(Delete), new { orderId = OrderDataFromDb.Id });
        }

        #region ApiCalls

        [HttpGet]

        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objorderHeaders; 

           

            if (User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Employee))
            {
                objorderHeaders = _unitOfWork.OrderHeader.GetAll(includeproperties: "ApplicationUser").ToList();
               
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objorderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, includeproperties: "ApplicationUser");

            }

            foreach (var item in objorderHeaders)
            {
                ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == item.ApplicationUserId);
                item.ApplicationUser = applicationUser;

            }

            switch (status)
                {
                    case "paymentpending":
                        objorderHeaders = objorderHeaders.Where(x => x.PaymentStatus == StaticDetails.PaymentStatusDelayedPayement);
                        break;

                    case "inprocess":
                        objorderHeaders = objorderHeaders.Where(x => x.OrderStatus == StaticDetails.OrderStatusInProcess);
                        break;

                    case "completed":
                        objorderHeaders = objorderHeaders.Where(x => x.OrderStatus == StaticDetails.OrderStatusShipped);
                        break;

                    case "approved":
                        objorderHeaders = objorderHeaders.Where(x => x.OrderStatus == StaticDetails.OrderStatusApproved);
                        break;

                    default:

                        break;
                }


            return Json(new { data = objorderHeaders });
        }


        #endregion
    }


}
