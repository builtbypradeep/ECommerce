using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MineCompanyController : Controller
    {
        //private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        
        public MineCompanyController( IUnitOfWork unitOfWork)
        {
            //_db = db;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Companies> obj = _unitOfWork.Comapany.GetAll().ToList();
            return View(obj);
        }

        public IActionResult Create()
        {
            return RedirectToAction("Index");
        }
    }
}
