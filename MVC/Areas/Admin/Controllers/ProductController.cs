using MaiCommerce.DataAccess.Repository.IRepository;
using MaiCommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace dotnetecommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductController(IUnitOfWork unitOfWork)
    {
        //Get the reference from the defined service in program.cs, DI
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        List<Product> productsList = _unitOfWork.Product.GetAll().ToList();
        return View(productsList);
    }

    public IActionResult Create()
    {
        //creating an IEnumerable and I'm using SelectListItem as the object
        //because then I can just create a select in the frontend for my dropdown
        //using IEnumerable for performance(since it send from server to client) and flexibility.
        IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category
            .GetAll()
            .Select(n => new SelectListItem
            {
                Text = n.Name,
                Value = n.Id.ToString()
            });
        //Put category list into a temporary storage from controller to view
        ViewBag.CategoryList = CategoryList;
        return View();
    }

    //Receiving the input(category obj) from client(), 
    //in the create view form and add it to database
    [HttpPost]
    public IActionResult Create(Product obj)
    {
        if (obj.Title == obj.Description)
        {
            ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
        }

        //Check if the value is validated from the 
        //based on object notation inside category model
        if (ModelState.IsValid)
        {
            //Track value and save it to db
            _unitOfWork.Product.Add(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product created successfully";
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

        Product? categoryFromDB = _unitOfWork.Product.Get(n => n.Id == id);
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
    public IActionResult Edit(Product obj)
    {
        //Check if the value is validated from the 
        //based on object notation inside category model
        if (ModelState.IsValid)
        {
            //Track value and update the one with the right id, primary key
            _unitOfWork.Product.Update(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product updated successfully";
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

        Product? productFromDB = _unitOfWork.Product.Get(n => n.Id == id);

        if (productFromDB == null)
        {
            return NotFound();
        }

        return View(productFromDB);
    }

    //Delete an entry in the database based on the Id in param
    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePOST(int? id)
    {
        //get row from db if exist
        Product? obj = _unitOfWork.Product.Get(n => n.Id == id);
        if (obj == null)
        {
            return NotFound();
        }

        //remove row from db
        _unitOfWork.Product.Remove(obj);
        _unitOfWork.Save();

        TempData["success"] = "Product deleted successfully";
        return RedirectToAction("Index");
    }
}