using Model;
using Database;
using dto;

namespace Controller;
public class UtilService
{

    WydDbContext db;
    AuthService authService;

    CommunityService communityService;
    public UtilService(WydDbContext context, AuthService authService, CommunityService communityService)
    {
        db = context;

        this.authService = authService;
        this.communityService = communityService;
    }

    public bool PopulateDb()
    {

        var transaction = db.Database.BeginTransaction();

        LoginDto first = new LoginDto("first@mail.com", "password", "first");
        authService.Register(first);
        LoginDto second = new LoginDto("second@mail.com", "password", "second");
        authService.Register(second);
        LoginDto third = new LoginDto("third@mail.com", "password", "third");
        authService.Register(third);
        LoginDto fourth = new LoginDto("fourth@mail.com", "password", "fourth");
        authService.Register(fourth);

        communityService.Create("1_2",[1,2]);
        communityService.Create("2_3",[2,3]);
        communityService.Create("2_4",[2,4]);
        communityService.Create("1_2_3_4",[1,2,3,4]);



        transaction.Commit();
        return true;
    }



}