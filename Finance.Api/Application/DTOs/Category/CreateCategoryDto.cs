using System.ComponentModel.DataAnnotations;

namespace Finance.Api.Application.DTOs.Category;

public class CreateCategoryDto{
    [Required]
    public string Name {get; set;} = String.Empty;
}