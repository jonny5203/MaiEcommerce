using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MaiCommerce.Models
{
    //This is a databse model of a catagory for Entity Framework
    //The annotation are the different contraint just like in a relational database
    public class Category
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be between 1-100")]
        public int DisplayOrder { get; set; }
    }
}