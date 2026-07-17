namespace Finance.Api.Application.DTOs.Categories;

public class CategoryDto
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public bool IsSystemCategory {get;set;}
}
