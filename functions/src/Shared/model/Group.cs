
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace Model;


[Table("Groups")]
public class Group : BaseEntity
{
    public string Hash { get; }  = CreateHashCode();

    public string Name { get; set; } = "";

    public bool GeneralForCommunity {get; set; } = true;

    [ForeignKey("CommunityId")]
    public virtual required Community Community { get; set; }

    [JsonIgnore]
    public virtual HashSet<User> Users { get; set; } = [];
    [JsonIgnore]
    public virtual List<UserGroup> UserGroups { get; set; } = [];


    private static string CreateHashCode()
    {
        byte[] randomBytes = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(randomBytes);
        string result = Convert.ToBase64String(randomBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
        return result;
    }
}