using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace Model;


[Table("Accounts")]
[Index(nameof(Mail), IsUnique = true)]
[Index(nameof(Uid), IsUnique = true)]
public class Account : BaseEntity
{
    public string Mail { get; set; } = string.Empty;
    public string Uid { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public required virtual User User {get; set;}
}