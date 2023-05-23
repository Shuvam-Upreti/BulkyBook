using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        [AutoValidateAntiforgeryToken]
        //Action
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        //upsert=update(Edit) + insert(Create)
        public IActionResult Upsert(int? Id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(
                i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }
                ),

                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(
               i => new SelectListItem
               {
                   Text = i.Name,
                   Value = i.Id.ToString()
               }
               ),
            };

            if (Id == null || Id == 0)
            {
                //create product
                //ViewBag.CategoryList = CategoryList; //viewbag procress
                //ViewData["CoverTypeList"] = CoverTypeList;
                return View(productVM);
            } 
            else
            {
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == Id);
                return View(productVM);
                //update product
            }

           
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;//get root path
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();//generating unique name of image file
                    var uploads = Path.Combine(wwwRootPath, @"images\products");//location where images is to be saved
                    var extension = Path.GetExtension(file.FileName);//getting extension of file

                    if(obj.Product.ImageUrl!=null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                            {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    //copying file from folder to inside project folder by using, USING STATEMENT
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);//fileStreams is the location where file is to be copied

                    }
                    //updating database after copying with location
                    obj.Product.ImageUrl = @"images\products\" + fileName + extension;

                }
                if (obj.Product.Id == 0) {
                    _unitOfWork.Product.Add(obj.Product);


                } 
                else {
                    _unitOfWork.Product.Update(obj.Product);
                }
                _unitOfWork.Save();
                TempData["success"] = "Created sucessfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

       
       

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(inculdeProperties:"Category,CoverType");
            return Json(new { data = productList });

        }
        //Post
        [HttpDelete]
        public IActionResult Delete(int? Id)
        {
            var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == Id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            //delete image
            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete sucessful" });
            
        }

        #endregion
    }
}
