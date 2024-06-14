using MaiCommerce.DataAccess.Data;
using MaiCommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace dotnetecommerce.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDBContext _db;
        public CategoryController(ApplicationDBContext db)
        {
            //Get the reference from the defined service in program.cs, DI
            _db = db;
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList();
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
                _db.Categories.Add(obj);
                _db.SaveChanges();
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

            Category? categoryFromDB = _db.Categories.Find(id);
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
                _db.Categories.Update(obj);
                _db.SaveChanges();
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

            Category? categoryFromDB = _db.Categories.Find(id);

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
            Category? obj = _db.Categories.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            //remove row from db
            _db.Categories.Remove(obj);
            _db.SaveChanges();

            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}