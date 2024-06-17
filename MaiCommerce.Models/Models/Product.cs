using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MaiCommerce.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
    
        [Required]
        public string Title { get; set; }
    
        public string Description { get; set; }
    
        [Required]
        public string ISBN { get; set; }
    
        [Required]
        public string Author { get; set; }
    
        [Required]
        [Display(Name = "List Price")]
        [Range(1, 1000)]
        public double ListPrice { get; set; }
    
        [Required]
        [Display(Name = "List Price 1-50 pieces")]
        [Range(1, 1000)]
        public double Price { get; set; }
    
        [Required]
        [Display(Name = "List Price 50-100 pieces")]
        [Range(1, 1000)]
        public double Price50 { get; set; }
    
        [Required]
        [Display(Name = "List Price 100+ pieces")]
        [Range(1, 1000)]
        public double Price100 { get; set; }
    }
}