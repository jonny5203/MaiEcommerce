using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MaiCommerce.Models.DataModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MaiCommerce.Models.IdentityModels;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string Name { get; set; }
    
    public string? StreetAddress { get; set; }
    
    public string? City { get; set; }
    
    public string? State { get; set; }
    
    public string? PostalCode { get; set; }
    
    // Foreign key for a Company using id
    public int? CompanyId { get; set; }
    
    // Foreign key object, and it should validated as it user doesn't have 
    // to be a company user(employee at a company)
    [ForeignKey("CompanyId")]
    [ValidateNever]
    public Company Company { get; set; }
}