using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models;
using ECommerce.utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize (Roles =StaticDetails.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public CategoryController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> objCategories = _UnitOfWork.Category.GetAll().ToList();
            return View(objCategories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                // _db.categories.Add(obj);
                //_db.SaveChanges();
                _UnitOfWork.Category.Add(obj);
                _UnitOfWork.Save();
                TempData["success"] = "Category created Succesfully.";
                return RedirectToAction("Index", "Category");
            }
            else
                return View();

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //Category? objdb = _db.categories.FirstOrDefault(x => x.Id == id);

            Category? objdb = _UnitOfWork.Category.Get(x => x.Id == id);

            if (objdb == null)
            {
                return NotFound();
            }

            return View(objdb);

        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                //_db.categories.Update(obj);
                //_db.SaveChanges();
                _UnitOfWork.Category.Update(obj);
                _UnitOfWork.Save();
                TempData["success"] = "Category updated Succesfully.";
                return RedirectToAction("Index");
            }
            else
                return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            //Category? objdb = _db.categories.FirstOrDefault(x => x.Id == id);

            Category? objdb = _UnitOfWork.Category.Get(x => x.Id == id);
            return View(objdb);

        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteRecord(int? id)
        {
            //Category? obj = _db.categories.FirstOrDefault(x => x.Id == id);

            Category? obj = _UnitOfWork.Category.Get(x => x.Id == id);

            if (obj == null || id == null || id == 0)
                return NotFound();
            else
            {
                //_db.Remove(obj);
                //_db.SaveChanges();
                _UnitOfWork.Category.Remove(obj);
                _UnitOfWork.Save();
                TempData["success"] = "Category deleted Succesfully.";
                return RedirectToAction("Index", "Category");
            }

        }

    }
}
