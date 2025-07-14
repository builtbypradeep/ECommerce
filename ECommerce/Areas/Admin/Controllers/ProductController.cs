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
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;

            //var appRoot = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.LastIndexOf("/bin") + 43 );

            //string path = Path.Combine(appRoot , "wwwroot");
          
            //_webHostEnvironment.WebRootPath = path;


        }

        //public ProductController(IUnitOfWork unitOfWork)
        //{
        //    _unitOfWork = unitOfWork;
        //}
        public IActionResult Index()
        {
            List<Product> objproducts = _unitOfWork.Product.GetAll(includeproperties:"Category").ToList();

            return View(objproducts);
        }

        //public IActionResult Create()
        //{

        //    //ViewBag.CategoryList = CategoryList;    
        //   // ViewData["CategoryList"] = CategoryList;

        //    ProductVM productVM = new()
        //    {
        //        CategoryList = _unitOfWork.Category.GetAll()
        //        .Select(u => new SelectListItem
        //        {
        //            Text = u.Name,
        //            Value = u.Id.ToString()
        //        }),
        //        product = new Product()
        //    };
        //    return View(productVM);
        //}

        public IActionResult Upsert(int? id)
        {

            //ViewBag.CategoryList = CategoryList;    
            // ViewData["CategoryList"] = CategoryList;

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                product = new Product()
            };

            if (id == null || id == 0)
            {
                //Create
                return View(productVM);
            }
            else
            {
                //update
                productVM.product = _unitOfWork.Product.Get( u => u.Id == id);
                return View(productVM);
            }


                
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string WebRootPath = _webHostEnvironment.WebRootPath;

                if (string.IsNullOrWhiteSpace(WebRootPath))
                {
                    WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }
                if (file != null)
                { 
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string ProductPath = Path.Combine(WebRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(obj.product.ImageUrl))
                    {
                        var oldimagepath = Path.Combine(WebRootPath, obj.product.ImageUrl.TrimStart('\\')); 

                        if (System.IO.File.Exists(oldimagepath))
                        {
                            System.IO.File.Delete(oldimagepath);
                        }
                    }
                    using (var filestream = new FileStream(Path.Combine(ProductPath, filename), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }

                    obj.product.ImageUrl = @"\images\product\" + filename;
                }

                if (obj.product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.product);
                    _unitOfWork.Save();
                    TempData["success"] = "Product created successfully.";
                }
                else
                {
                    _unitOfWork.Product.Update(obj.product);
                    _unitOfWork.Save();
                    TempData["success"] = "Product updated successfully.";
                }

                   
               
                return RedirectToAction("Index");
            }
            return View();
        }
        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //        return NotFound();

        //    Product? obj = _unitOfWork.Product.Get(x => x.Id == id);

        //    if (obj == null)
        //        return NotFound();


        //    return View(obj);
        //}

        //[HttpPost]
        //public IActionResult Edit(Product product)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(product);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product updated successfully.";
        //    }


        //    return RedirectToAction("Index");
        //}

        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //        return NotFound();

        //    Product obj = _unitOfWork.Product.Get(x => x.Id == id);

        //    if (obj == null)
        //        return NotFound();
        //    return View(obj);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeleteRecord(int? id)
        //{
        //    Product obj = _unitOfWork.Product.Get(x => x.Id == id);

        //    if (obj == null || id == null || id == 0)
        //        return NotFound();

        //    _unitOfWork.Product.Remove(obj);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Product deleted successfully.";
        //    return RedirectToAction("Index");
        //}S

        #region ApiCalls

        [HttpGet]

        public IActionResult GetAll()
        {
            List<Product> objproducts = _unitOfWork.Product.GetAll(includeproperties: "Category").ToList();

            return Json(new { data = objproducts });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            //List<Product> objproducts = _unitOfWork.Product.GetAll(includeproperties: "Category").ToList();

            //return Json(new { data = objproducts });

            var ProductTobeDeleted = _unitOfWork.Product.Get(u => u.Id == id);

            if (ProductTobeDeleted == null)
            {
                return Json(new {success = false, message = "Error while deleting"});
            }
            else
            {
                var ImageTobeDeleted = Path.Combine(_webHostEnvironment.WebRootPath, ProductTobeDeleted.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(ImageTobeDeleted))
                {
                    System.IO.File.Delete(ImageTobeDeleted); 
                }

                _unitOfWork.Product.Remove(ProductTobeDeleted);
                _unitOfWork.Save();

                return Json(new { success = true, message = "Product Deleted." });
            }
        }



        #endregion
    }


}
