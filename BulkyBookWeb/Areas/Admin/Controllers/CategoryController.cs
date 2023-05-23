using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers   
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)] 
    public class CategoryController : Controller
    {
        //for creating object using dependency injection
        // cretae private readonly field
        private readonly IUnitOfWork _unitOfWork;

        //constructor
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            //with the help of _db we acess database and retrive data into the code
            //var ko satta ma Ienumerable use garyaa 

            IEnumerable<Category> objCategoryList = _unitOfWork.Category.GetAll();
            //multiple data ko laagi loop run garnu parxa ra tyo view objcategorylist ma return huna parxa
            return View(objCategoryList);
        }
        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Display order cannot match name.");
            }
            if (ModelState.IsValid)
            {

                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Created sucessfully";
                return RedirectToAction("Index");

            }
            return View(obj);
        }

        //GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //var categoryFromDb = _db.Categories.Find(id);
            var categoryFromDbFirst = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);


            if (categoryFromDbFirst == null)
            {
                return NotFound();
            }
            return View(categoryFromDbFirst);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Display order cannot match name.");
            }
            if (ModelState.IsValid)
            {

                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Edited sucessfully";
                return RedirectToAction("Index");

            }
            return View(obj);
        }

        //GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //var categoryFromDb = _db.Categories.Find(id);
            var categoryFromDbFirst = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);


            if (categoryFromDbFirst == null)
            {
                return NotFound();
            }
            return View(categoryFromDbFirst);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Deleted sucessfully";
            return RedirectToAction("Index");

        }

    }
}
