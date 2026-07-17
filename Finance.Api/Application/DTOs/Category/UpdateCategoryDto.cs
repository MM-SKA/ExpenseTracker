using System.ComponentModel.DataAnnotations;

namespace Finance.Api.Application.DTOs.Category;

public class UpdateCategoryDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
