using ECommerce.DataAccess.Repository;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Areas.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIndentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIndentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claims != null)
            {
                if (HttpContext.Session.GetInt32(StaticDetails.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claims.Value).Count());
                }
                return View(HttpContext.Session.GetInt32(StaticDetails.SessionCart));
            }
            else
            {
                return View(0);
            }
        }
    }
}
