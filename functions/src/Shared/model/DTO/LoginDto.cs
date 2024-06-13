namespace dto;

public class LoginDto
{
    public string Mail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;

    public LoginDto(string mail, string password, string? username){
        this.Mail = mail;
        this.Password = password;
        this.Username = username ?? string.Empty;
    }
}