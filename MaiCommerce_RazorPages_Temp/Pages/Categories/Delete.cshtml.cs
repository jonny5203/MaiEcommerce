using MaiCommerce_RazorPages_Temp.Data;
using MaiCommerce_RazorPages_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaiCommerce_RazorPages_Temp.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDBContext _db;
        [BindProperty]
        public Category category { get; set; }

        public DeleteModel(ApplicationDBContext db)
        {
            _db = db;
        }

        public void OnGet(int? Id)
        {
            if (Id != null || Id != 0)
            {
                category = _db.Categories.Find(Id);
            }
        }

        public IActionResult OnPost()
        {
            if (category != null)
            {
                //Track value and update the one with the right id, primary keyy
                _db.Categories.Remove(category);
                _db.SaveChanges();
                TempData["success"] = "Category Deleted successfully";
            }

            return RedirectToPage("Index");
        }
    }
}