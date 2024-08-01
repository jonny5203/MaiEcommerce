using System.Diagnostics;
using System.Security.Claims;
using MaiCommerce.DataAccess.Repository.IRepository;
using MaiCommerce.Models.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnetecommerce.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    
    private readonly ILogger<HomeController> _logger;
    public readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
        return View(productList);
    }
    
    public IActionResult Details(int productId)
    {
        ShoppingCart shoppingCart = new()
        {
            Product = _unitOfWork.Product.Get(n => n.Id == productId, includeProperties: "Category"),
            Count = 1,
            ProductId = productId
        };
        
        return View(shoppingCart);
    }
    
    [HttpPost]
    // Saying that the user have to be authorized
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        // Helper method for getting the logged in users id
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        
        // Assigning userId to cart obj, and add it to DB
        shoppingCart.ApplicationUserId = userId;
        
        ShoppingCart cartFromDB = _unitOfWork.ShoppingCart.Get(n => n.ApplicationUserId == userId 
                                                                    && n.ProductId == shoppingCart.ProductId);
        if (cartFromDB != null)
        {
            //shopping cart exist
            //this line is still being tracked by ef core and saved to the database
            //event though the Add function wasn't called, I guess it tracks what is happening with
            //the reference automatically
            cartFromDB.Count += shoppingCart.Count;
            _unitOfWork.ShoppingCart.Update(cartFromDB);
        }
        else
        {
            //add to cart
            _unitOfWork.ShoppingCart.Add(shoppingCart);
        }
        
        TempData["success"] = "Cart Updated Successfully";
        _unitOfWork.Save();
        
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
