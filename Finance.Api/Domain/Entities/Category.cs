using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Finance.Api.Models;

public class Category{
    public int Id {get; set;}

    [Required]
    [MaxLength(50)]
    public required string Name {get; set;}

    [JsonIgnore]
    public ICollection<Expense> Expenses {get; set;} = new List<Expense>();

    public int? UserId { get; set; }

    public bool IsSystemCategory {get;set;}

    [JsonIgnore]
    public AppUser? User { get; set; }

}