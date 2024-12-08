using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Model;

[Index(nameof(Hash), IsUnique = true)]
public class BaseHashEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [JsonIgnore]
    public virtual DateTime CreatedAt { get; set; }
    [JsonIgnore]
    public virtual DateTime UpdatedAt { get; set; }
    
    public string Hash { get; set; } = CreateHashCode();

    private static string CreateHashCode()
    {
        byte[] randomBytes = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(randomBytes);
        string result = Convert.ToBase64String(randomBytes)
            .Replace('+', '-')
            .Replace("/", "")
            .Replace("=", "");
        return result;
    }
}