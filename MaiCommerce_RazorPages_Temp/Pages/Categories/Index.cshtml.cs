using MaiCommerce_RazorPages_Temp.Data;
using MaiCommerce_RazorPages_Temp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MaiCommerce_RazorPages_Temp.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDBContext _db;
        public List<Category> CategoryList { get; set;}

        public IndexModel(ApplicationDBContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            CategoryList = _db.Categories.ToList();
        }
    }
}