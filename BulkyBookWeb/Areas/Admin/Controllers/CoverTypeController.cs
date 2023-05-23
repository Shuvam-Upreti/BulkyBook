using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [AutoValidateAntiforgeryToken]
        //Action
        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<CoverType> objCoverTypeList = _unitOfWork.CoverType.GetAll();
            return View(objCoverTypeList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CoverType obj)
        {
            if(obj.Id.ToString()==obj.Name)
            {
                ModelState.AddModelError("name", "Id cant mathch name");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Created sucessfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Edit(int? Id)
        {
            if(Id == null || Id==0)
            {
                return NotFound();
            }
            var coverTypeFromDb = _unitOfWork.CoverType.GetFirstOrDefault(u=>u.Id==Id);

            if(coverTypeFromDb == null)
            {
                return NotFound();
            }
            return View (coverTypeFromDb);
        }
        [HttpPost]
        public IActionResult Edit(CoverType obj)
        {
            _unitOfWork.CoverType.Update(obj);
            _unitOfWork.Save();
            TempData["success"] = "Edited sucessfully";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var coverTypeFromDb = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == Id);

            if (coverTypeFromDb == null)
            {
                return NotFound();
            }
            return View(coverTypeFromDb);
        }
        [HttpPost]
        public IActionResult DeletePOST(int? Id)
        {
            var obj = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == Id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.CoverType.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Deleted sucessfully";
            return RedirectToAction("Index");
        }
    }
}
