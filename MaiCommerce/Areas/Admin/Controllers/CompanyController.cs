using MaiCommerce.DataAccess.Repository.IRepository;
using MaiCommerce.Models.DataModels;
using MaiCommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnetecommerce.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = SD.Role_Admin)]
public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyController(IUnitOfWork unitOfWork)
    {
        //Get the reference from the defined service in program.cs, DI
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        List<Company> CompanysList = _unitOfWork.Company.GetAll().ToList();
        return View(CompanysList);
    }

    public IActionResult Upsert(int? id)
    {
        if (id == null || id == 0)
        {
            //create
            return View(new Company());
        }
        else
        {
            //update
            Company companyObj = _unitOfWork.Company.Get(n => n.Id == id);
            return View(companyObj);
        }
    }

    //Receiving the input(category obj) from client(), 
    //in the create view form and add it to database
    [HttpPost]
    public IActionResult Upsert(Company companyObj)
    {
        //Check if the value is validated from the 
        //based on object notation inside category model
        if (ModelState.IsValid)
        {
            if (companyObj.Id == 0)
            {
                //Track value and save the Company to db
                _unitOfWork.Company.Add(companyObj);
            }
            else
            {
                _unitOfWork.Company.Update(companyObj);
            }
            
            _unitOfWork.Save();
            TempData["success"] = "Company created successfully";
            return RedirectToAction("Index");
        }

        return View(companyObj);
    }
    
    //Creating api endpoints inside Company controller navigatiion route
    #region API CALLS
    
    //API endpoint for getting the Company list as a json object
    [HttpGet]
    public IActionResult GetAll()
    {
        List<Company> objCompany = _unitOfWork.Company.GetAll().ToList();

        return Json(new { data = objCompany });
    }
    
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        Company CompanyToBeDeleted = _unitOfWork.Company.Get(n => n.Id == id);
        if (CompanyToBeDeleted == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }
        
        //remove entry from db
        _unitOfWork.Company.Remove(CompanyToBeDeleted);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Deleted Successfully" });
    }
    
    #endregion
}