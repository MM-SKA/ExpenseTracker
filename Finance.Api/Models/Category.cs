using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Finance.Api.Models;

public class Category{
    public int Id {get; set;}

    [Required]
    [MaxLength(50)]
    public string Name {get; set;} = String.Empty;

    [JsonIgnore]
    public ICollection<Expense> Expenses {get; set;} = new List<Expense>();

    public int UserId { get; set; }

    [JsonIgnore]
    public AppUser? User { get; set; }

}