

using System.Collections;
using System.Runtime.CompilerServices;
using Database;
using Dto;
using FirebaseAdmin.Auth;
using Model;

namespace Service;
public class UtilService
{

    WydDbContext db;
    AuthService authService;
    UserService userService;
    CommunityService communityService;

    public UtilService(WydDbContext context, AuthService authService, UserService userService, CommunityService communityService)
    {
        db = context;

        this.authService = authService;
        this.userService = userService;
        this.communityService = communityService;
    }

    public async Task<bool> InitDb()
    {
        List<string> mails = ["prova", "prova1", "prova2", "prova3"];

        List<User> users = await CreateUsersAsync(mails);
        List<UserDto> userDtos = users.Select(user => new UserDto(user)).ToList();

        List<Tuple<CreateCommunityDto, User>> normalPairs = GeneratePersonalAndSingleGroupCommunities(users, userDtos);
        List<Tuple<CreateCommunityDto, User>> multiplePairs = GenerateMultipleGroupCommunities(users, userDtos);

        List<Community> normalCommunities = normalPairs.Select(pair => communityService.Create(pair.Item1, pair.Item2)).ToList();
        List<Community> multipleCommunities = multiplePairs.Select(pair => communityService.Create(pair.Item1, pair.Item2)).ToList();

        try
        {
            multipleCommunities.ForEach(c => communityService.MakeMultiGroup(c));

            GroupDto group = new()
            {
                Name = "Secondary Group",
                GeneralForCommunity = false,
                Users = [userDtos[0], userDtos[1], userDtos[2]]
            };

            communityService.CreateAndAddNewGroup(multipleCommunities[1], group, users[0]);

            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error initializing database entities.", ex);
        }
    }

    private async Task<List<User>> CreateUsersAsync(ICollection<string> mails)
    {
        List<User> users = [];
        foreach (var mail in mails)
        {
            users.Add(await CreateNewUserAsync(mail + "@mail.com", "password"));
        }
        return users;
    }

    private async Task<User> CreateNewUserAsync(string mail, string password)
    {
        try
        {
            string uid = await CreateOrRetrieveFirebaseUserUidWithEmailAndPassword(mail, password);
            return await userService.GetOrCreateAsync(uid);
        }
        catch (Exception e)
        {
            throw new Exception("Error with user creation: " + e.Message);
        }
    }

    private static async Task<string> CreateOrRetrieveFirebaseUserUidWithEmailAndPassword(string email, string password)
    {

        UserRecordArgs userRecordArgs = new()
        {
            Email = email,
            EmailVerified = false,
            Password = password,
            DisplayName = "WydUser",
            Disabled = false,
        };

        UserRecord userRecord;
        try
        {
            userRecord = await AuthService.CreateUserAsync(userRecordArgs);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("EMAIL_EXISTS"))
                userRecord = await AuthService.RetrieveFirebaseUserFromMail(email);
            else
                throw new Exception("Error creating Firebase user: " + ex.Message);
        }
        return userRecord.Uid;
    }

    private static List<Tuple<CreateCommunityDto, User>> GeneratePersonalAndSingleGroupCommunities(List<User> users, List<UserDto> userDtos)
    {
        List<Tuple<CreateCommunityDto, User>> pairs = [];

        // Personal Community

        pairs.Add(
            new Tuple<CreateCommunityDto, User>(
                new CreateCommunityDto()
                {
                    Id = 0,
                    Name = "Personal Community 1 2",
                    Type = CommunityType.Personal,
                    Users = [userDtos[0], userDtos[1]]
                },
                users[0]
            )
        );

        pairs.Add(
            new Tuple<CreateCommunityDto, User>(
                new CreateCommunityDto()
                {
                    Id = 0,
                    Name = "Personal Community 1 3",
                    Type = CommunityType.Personal,
                    Users = [userDtos[0], userDtos[2]]
                },
                users[0]
            )
        );

        pairs.Add(
            new Tuple<CreateCommunityDto, User>(
                new CreateCommunityDto()
                {
                    Id = 0,
                    Name = "Personal Community 1 4",
                    Type = CommunityType.Personal,
                    Users = [userDtos[0], userDtos[3]]
                },
                users[0]
            )
        );

        pairs.Add(
            new Tuple<CreateCommunityDto, User>(
                new CreateCommunityDto()
                {
                    Id = 0,
                    Name = "Personal Community 2 4",
                    Type = CommunityType.Personal,
                    Users = [userDtos[1], userDtos[3]]
                },
                users[3]
            )
        );

        pairs.Add(
            new Tuple<CreateCommunityDto, User>(
                new CreateCommunityDto()
                {
                    Id = 0,
                    Name = "Personal Community 3 4",
                    Type = CommunityType.Personal,
                    Users = [userDtos[2], userDtos[3]]
                },
                users[3]
            )
        );

        // Single Group Community
        pairs.Add(
            new Tuple<CreateCommunityDto, User>(
                new CreateCommunityDto()
                {
                    Id = 0,
                    Name = "Single Group 1_2",
                    Type = CommunityType.SingleGroup,
                    Users = [userDtos[0], userDtos[1]]
                },
                users[0]
            )
        );



        pairs.Add(
            new Tuple<CreateCommunityDto, User>(
                new CreateCommunityDto()
                {
                    Id = 0,
                    Name = "Single Group 1_2_3_4",
                    Type = CommunityType.SingleGroup,
                    Users = [userDtos[0], userDtos[1], userDtos[2], userDtos[3]]
                },
                users[0]
            )
        );

        return pairs;
    }

    private static List<Tuple<CreateCommunityDto, User>> GenerateMultipleGroupCommunities(List<User> users, List<UserDto> userDtos)
    {
        List<Tuple<CreateCommunityDto, User>> pairs = [];

        // General Community
        pairs.Add(
            new Tuple<CreateCommunityDto, User>(
                new CreateCommunityDto()
                {
                    Id = 0,
                    Name = "General Community",
                    Type = CommunityType.SingleGroup,
                    Users = [userDtos[0], userDtos[1], userDtos[2]]
                },
                users[0]
            )
        );

        pairs.Add(
            new Tuple<CreateCommunityDto, User>(
                new CreateCommunityDto()
                {
                    Id = 0,
                    Name = "General Community 1",
                    Type = CommunityType.SingleGroup,
                    Users = [userDtos[0], userDtos[1], userDtos[2], userDtos[3]]
                },
                users[0]
            )
        );

        return pairs;
    }


}