using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaiCommerce_RazorPages_Temp.Data;
using MaiCommerce_RazorPages_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MaiCommerce_RazorPages_Temp.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDBContext _db;

        [BindProperty]
        public Category category { get; set; }

        public EditModel(ApplicationDBContext db)
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

        //Update values in an existing row in the database
        public IActionResult OnPost()
        {

            if (ModelState.IsValid)
            {
                //Track value and update the one with the right id, primary key
                _db.Categories.Update(category);
                _db.SaveChanges();
                TempData["success"] = "Category updated successfully";
                return RedirectToPage("Index");
            }

            return Page();
        }
    }
}