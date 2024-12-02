using Model;

public class AccountDto (Account account){
    public int Id {get; set; }= account.Id;
    public string Mail { get; set; } = account.Mail;
    public string Uid { get; set; } = account.Uid;
}