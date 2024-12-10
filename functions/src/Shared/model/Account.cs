using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;


namespace Model;


[Table("Accounts")]
[Index(nameof(Mail), IsUnique = true)]
[Index(nameof(Uid), IsUnique = true)]
public class Account : BaseEntity
{
    public required string Mail { get; set; }
    public required string Uid { get; set; }

    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual required User User { get; set;}
}