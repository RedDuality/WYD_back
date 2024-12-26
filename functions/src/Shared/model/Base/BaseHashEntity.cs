using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace Model;

//baseEntity is checked for automatic dates updates, check dbContext SaveState
[Index(nameof(Hash), IsUnique = true)]
public class BaseHashEntity : BaseEntity
{
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