using System.ComponentModel.DataAnnotations;

namespace Finance.Api.DTOs.Category;

public class CreateCategoryDto{
    [Required]
    public string Name {get; set;} = String.Empty;
}