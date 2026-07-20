using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Finance.Api.Domain.Entities;

internal class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    [JsonIgnore]
    public ICollection<Expense> Expenses { get; } = [];

    public int? UserId { get; set; }

    public bool IsSystemCategory { get; set; }

    [JsonIgnore]
    public AppUser? User { get; set; }
}
