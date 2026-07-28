namespace Finance.Api.Application.DTOs.Category;

public class CategoryDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }

    public bool IsSystemCategory { get; set; }
}
