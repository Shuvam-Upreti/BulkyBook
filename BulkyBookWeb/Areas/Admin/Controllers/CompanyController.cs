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

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            Company company = new();
            

            if (Id == null || Id == 0)
            {
                return View(company);
            } 
            else
            {
                company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == Id);
                return View(company);
            }

           
        }
        [HttpPost]
        public IActionResult Upsert(Company obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
               
                if (obj.Id == 0) {
                    _unitOfWork.Company.Add(obj);
                    TempData["success"] = "Created sucessfully";
                }
                else {
                    _unitOfWork.Company.Update(obj);
                    TempData["success"] = "Updated sucessfully";

                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

       
       

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var companyList = _unitOfWork.Company.GetAll();
            return Json(new { data = companyList });

        }
        //Post
        [HttpDelete]
        public IActionResult Delete(int? Id)
        {
            var obj = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == Id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete sucessful" });
            
        }
         
        #endregion
    }
}
