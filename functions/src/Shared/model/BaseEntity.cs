using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [JsonIgnore]
    public virtual DateTime CreatedAt { get; set; }
    [JsonIgnore]
    public virtual DateTime UpdatedAt { get; set; }
}