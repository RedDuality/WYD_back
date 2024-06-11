using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;


namespace Model;


[Table("Users")]
[Index(nameof(mail), IsUnique = true)]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string mail { get; set; } = string.Empty;
    public string username { get; set; } = string.Empty;

    [JsonIgnore]
    public byte[] PasswordHash { get; set; } = [];
    [JsonIgnore]
    public byte[] PasswordSalt { get; set; } = [];


    [JsonIgnore]
    public virtual List<Event> Events { get; set; } = [];
    [JsonIgnore]
    public virtual List<UserEvent> UserEvents { get; set; } = [];
    [JsonIgnore]
    public virtual List<Community> Communities { get; set; } = [];
}