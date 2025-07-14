using ECommerce.DataAccess.Repository;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models;
using ECommerce.Models.ViewModels;
using ECommerce.utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Plugins;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;


namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize (Roles =StaticDetails.Role_Admin)]
    public class ComapnyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        //private readonly IWebHostEnvironment _webHostEnvironment;

        public ComapnyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            //_webHostEnvironment = webHostEnvironment;

            //var appRoot = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.LastIndexOf("/bin") + 43 );

            //string path = Path.Combine(appRoot , "wwwroot");
          
            //_webHostEnvironment.WebRootPath = path;


        }

        //public ComapnyController(IUnitOfWork unitOfWork)
        //{
        //    _unitOfWork = unitOfWork;
        //}
        public IActionResult Index()
        {
            List<Companies> objCompanys = _unitOfWork.Comapany.GetAll().ToList();

            return View(objCompanys);
        }

        //public IActionResult Create()
        //{

        //    //ViewBag.CategoryList = CategoryList;    
        //   // ViewData["CategoryList"] = CategoryList;

        //    CompanyVM CompanyVM = new()
        //    {
        //        CategoryList = _unitOfWork.Category.GetAll()
        //        .Select(u => new SelectListItem
        //        {
        //            Text = u.Name,
        //            Value = u.Id.ToString()
        //        }),
        //        Company = new Company()
        //    };
        //    return View(CompanyVM);
        //}

        public IActionResult Upsert(int? id)
        {

            //ViewBag.CategoryList = CategoryList;    
            // ViewData["CategoryList"] = CategoryList;

            //CompanyVM CompanyVM = new()
            //{
            //    CategoryList = _unitOfWork.Category.GetAll()
            //    .Select(u => new SelectListItem
            //    {
            //        Text = u.Name,
            //        Value = u.Id.ToString()
            //    }),
            //    companies = new Companies()
            //};

            if (id == null || id == 0)
            {
                //Create
                return View(new Companies());
            }
            else
            {
                //update
                //CompanyVM.companies = _unitOfWork.Comapany.Get( u => u.Id == id);

                Companies  objcompanies = _unitOfWork.Comapany.Get(u => u.Id == id);
                return View(objcompanies);
            }


                
        }

        [HttpPost]
        public IActionResult Upsert(Companies obj)
        {
            if (ModelState.IsValid)
            {
                //string WebRootPath = _webHostEnvironment.WebRootPath;

                //if (string.IsNullOrWhiteSpace(WebRootPath))
                //{
                //    WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                //}
                //if (file != null)
                //{ 
                //    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                //    string CompanyPath = Path.Combine(WebRootPath, @"images\Company");

                    //if (!string.IsNullOrEmpty(obj.companies.ImageUrl))
                    //{
                    //    var oldimagepath = Path.Combine(WebRootPath, obj.Company.ImageUrl.TrimStart('\\')); 

                    //    if (System.IO.File.Exists(oldimagepath))
                    //    {
                    //        System.IO.File.Delete(oldimagepath);
                    //    }
                    //}
                    //using (var filestream = new FileStream(Path.Combine(CompanyPath, filename), FileMode.Create))
                    //{
                    //    file.CopyTo(filestream);
                    //}

                    //obj.Company.ImageUrl = @"\images\Company\" + filename;
               // }

                if (obj.Id == 0)
                {
                    _unitOfWork.Comapany.Add(obj);
                    _unitOfWork.Save();
                    TempData["success"] = "Company created successfully.";
                }
                else
                {
                    _unitOfWork.Comapany.Update(obj);
                    _unitOfWork.Save();
                    TempData["success"] = "Company updated successfully.";
                }

                   
               
                return RedirectToAction("Index");
            }
            return View();
        }
        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //        return NotFound();

        //    Company? obj = _unitOfWork.Company.Get(x => x.Id == id);

        //    if (obj == null)
        //        return NotFound();


        //    return View(obj);
        //}

        //[HttpPost]
        //public IActionResult Edit(Company Company)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Company.Update(Company);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Company updated successfully.";
        //    }


        //    return RedirectToAction("Index");
        //}

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            Companies obj = _unitOfWork.Comapany.Get(x => x.Id == id);

            if (obj == null)
                return NotFound();
            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteRecord(int? id)
        {
            Companies obj = _unitOfWork.Comapany.Get(x => x.Id == id);

            if (obj == null || id == null || id == 0)
                return NotFound();

            _unitOfWork.Comapany.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Company deleted successfully.";
            return RedirectToAction("Index");
        }
        

        #region ApiCalls

        [HttpGet]

        public IActionResult GetAll()
        {
            List<Companies> objCompanys = _unitOfWork.Comapany.GetAll().ToList();

            return Json(new { data = objCompanys });
        }

        //[HttpDelete]
        //public IActionResult Delete(int? id)
        //{
        //    //List<Company> objCompanys = _unitOfWork.Company.GetAll(includeproperties: "Category").ToList();

        //    //return Json(new { data = objCompanys });

        //    var CompanyTobeDeleted = _unitOfWork.Comapany.Get(u => u.Id == id);

        //    if (CompanyTobeDeleted == null)
        //    {
        //        return Json(new {success = false, message = "Error while deleting"});
        //    }
        //    else
        //    {
        //        //var ImageTobeDeleted = Path.Combine(_webHostEnvironment.WebRootPath, CompanyTobeDeleted.ImageUrl.TrimStart('\\'));

        //        //if (System.IO.File.Exists(ImageTobeDeleted))
        //        //{
        //        //    System.IO.File.Delete(ImageTobeDeleted); 
        //        //}

        //        _unitOfWork.Comapany.Remove(CompanyTobeDeleted);
        //        _unitOfWork.Save();

        //        return Json(new { success = true, message = "Company Deleted." });
        //    }
        //}



        #endregion
    }


}
