using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Finance.Api.Domain.Entities;

public class Category
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    [JsonIgnore]
    public ICollection<Expense> Expenses { get; } = [];

    public string UserId { get; set; } = string.Empty;

    public bool IsSystemCategory { get; set; }

    [JsonIgnore]
    public AppUser? User { get; set; }
}
