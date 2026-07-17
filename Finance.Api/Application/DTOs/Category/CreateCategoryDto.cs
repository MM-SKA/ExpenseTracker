using System.ComponentModel.DataAnnotations;

namespace Finance.Api.Application.DTOs.Category;

public class CreateCategoryDto
{
    [Required]
    public required string Name { get; set; }
}