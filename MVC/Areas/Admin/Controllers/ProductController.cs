using MaiCommerce.DataAccess.Repository.IRepository;
using MaiCommerce.Models;
using MaiCommerce.Models.ViewModels;
using MaiCommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace dotnetecommerce.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        //Get the reference from the defined service in program.cs, DI
        _unitOfWork = unitOfWork;
        //instance to access the hosting environment, wwwroot folder
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        List<Product> productsList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
        return View(productsList);
    }

    public IActionResult Upsert(int? id)
    {
        //Put category list into a temporary storage from controller to view
        //ViewData["CategoryList"] = CategoryList;

        //Init and pass viewmodel instead for databinding and strongly typed type
        ProductVM productVm = new()
        {
            //creating an IEnumerable and I'm using SelectListItem as the object
            //because then I can just create a select in the frontend for my dropdown
            //using IEnumerable for performance(since it send from server to client) and flexibility.
            CategoryList = _unitOfWork.Category
                .GetAll()
                .Select(n => new SelectListItem
                {
                    Text = n.Name,
                    Value = n.Id.ToString()
                }),
            Product = new Product()
        };

        if (id == null || id == 0)
        {
            //create
            return View(productVm);
        }
        else
        {
            //update
            productVm.Product = _unitOfWork.Product.Get(n => n.Id == id);
            return View(productVm);
        }
    }

    //Receiving the input(category obj) from client(), 
    //in the create view form and add it to database
    [HttpPost]
    public IActionResult Upsert(ProductVM productVm, IFormFile? file)
    {
        //Check if the value is validated from the 
        //based on object notation inside category model
        if (ModelState.IsValid)
        {
            //getting the wwwroot path and check if exist
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                //create a file with a GUID as name, and with the same file extention as
                //the file uploaded. Then also combine the wwwRootPath, with the folder path
                //within wwwroot that the program is going to save into
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                
                //Because I'm developing on linux, the path is /, but on windows the path is \
                string productPath = Path.Combine(wwwRootPath, @"images/product");

                if (!string.IsNullOrEmpty(productVm.Product.ImageUrl))
                {
                    //delete the old image
                    var oldImagePath = Path.Combine(wwwRootPath, productVm.Product.ImageUrl.TrimStart('/'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                
                //Automatic disposable file for saving image to folder
                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                
                //write the path to the image to the model
                productVm.Product.ImageUrl = @"/images/product/" + fileName;
            }

            if (productVm.Product.Id == 0)
            {
                //Track value and save the product to db
                _unitOfWork.Product.Add(productVm.Product);
            }
            else
            {
                _unitOfWork.Product.Update(productVm.Product);
            }
            
            _unitOfWork.Save();
            TempData["success"] = "Product created successfully";
            return RedirectToAction("Index");
        }
        else
        {
            //fixing the exception where the dropdown will be
            //empty if something refreshes
            productVm.CategoryList = _unitOfWork.Category
                .GetAll()
                .Select(n => new SelectListItem
                {
                    Text = n.Name,
                    Value = n.Id.ToString()
                });

            return View(productVm);
        }
    }
    
    //Creating api endpoints inside product controller navigatiion route
    #region API CALLS
    
    //API endpoint for getting the product list as a json object
    [HttpGet]
    public IActionResult GetAll()
    {
        List<Product> objProduct = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

        return Json(new { data = objProduct });
    }
    
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        Product productToBeDeleted = _unitOfWork.Product.Get(n => n.Id == id);
        if (productToBeDeleted == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }
        
        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('/'));
        
        // Delete the image in wwwroot/image folder
        // TODO: try catch statement to catch error if something were to happen other than that file exist
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }
        
        //remove entry from db
        _unitOfWork.Product.Remove(productToBeDeleted);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Deleted Successfully" });
    }
    
    #endregion
}