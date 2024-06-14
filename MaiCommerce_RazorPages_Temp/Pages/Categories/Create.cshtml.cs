using MaiCommerce_RazorPages_Temp.Data;
using MaiCommerce_RazorPages_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaiCommerce_RazorPages_Temp.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDBContext _db;
        [BindProperty]
        public Category CategoryObj { get; set;}

        public CreateModel(ApplicationDBContext db)
        {
            _db = db;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            _db.Categories.Add(CategoryObj);
            _db.SaveChanges();
            TempData["success"] = "Category Created Successfully";
            return RedirectToPage("Index");
        }
    }
}