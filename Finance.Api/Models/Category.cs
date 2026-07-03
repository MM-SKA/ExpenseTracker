using System.ComponentModel.DataAnnotations;

namespace Finance.Api.Models;

public class Category{
    public int Id {get; set;}

    [Required]
    [MaxLength(50)]
    public string Name {get; set;} = String.Empty;

    public ICollection<Expense> Expenses {get; set;} = new List<Expense>();

    public int UserId { get; set; }

    public AppUser? User { get; set; }

}