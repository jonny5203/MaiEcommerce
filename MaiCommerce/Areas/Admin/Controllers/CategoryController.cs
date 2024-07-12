using MaiCommerce.DataAccess.Repository.IRepository;
using MaiCommerce.Models.DataModels;
using MaiCommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnetecommerce.Areas.Admin.Controllers
{
    //Area annotation for the controller to know which area it is in
    //without it you will either get an exception on the page or a blanc page
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    //only admin accounts can access this controller
    //[Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            //Get the reference from the defined service in program.cs, DI
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        //Receiving the input(category obj) from client(), 
        //in the create view form and add it to database
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
            }

            //Check if the value is validated from the 
            //based on object notation inside category model
            if (ModelState.IsValid)
            {
                //Track value and save it to db
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? categoryFromDB = _unitOfWork.Category.Get(n => n.Id == id);
            //Category? categoryFromDB1 = _db.Categories.FirstOrDefault(u => u.Id == id);
            //Category? categoryFromDB2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

            if (categoryFromDB == null)
            {
                return NotFound();
            }

            return View(categoryFromDB);
        }

        //Update values in an existing row in the database
        [HttpPost]
        public IActionResult Edit(Category obj)
        {


            //Check if the value is validated from the 
            //based on object notation inside category model
            if (ModelState.IsValid)
            {
                //Track value and update the one with the right id, primary key
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? categoryFromDB = _unitOfWork.Category.Get(n => n.Id == id);

            if (categoryFromDB == null)
            {
                return NotFound();
            }

            return View(categoryFromDB);
        }

        //Delete an entry in the database based on the Id in param
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            //get row from db if exist
            Category? obj = _unitOfWork.Category.Get(n => n.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            //remove row from db
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();

            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}