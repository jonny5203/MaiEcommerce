using System.Diagnostics;
using MaiCommerce.DataAccess.Repository;
using MaiCommerce.DataAccess.Repository.IRepository;
using MaiCommerce.Models;
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
    
    public IActionResult Details(int id)
    {
        Product productList = _unitOfWork.Product.Get(n => n.Id == id, includeProperties: "Category");
        return View(productList);
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
