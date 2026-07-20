namespace Finance.Api.Application.DTOs.Category;

internal class CategoryDto
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public bool IsSystemCategory { get; set; }
}
