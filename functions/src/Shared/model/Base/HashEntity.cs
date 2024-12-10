
using Microsoft.EntityFrameworkCore;
using Service;

namespace Model;


[Index(nameof(Hash), IsUnique = true)]
public class HashEntity
{
    public string Hash { get; set; } = HashService.CreateHashCode();

}