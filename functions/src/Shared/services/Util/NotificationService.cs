
using Google.Cloud.Firestore;
using Model;

namespace Service;

public enum UdpateType
{
    NewEvent,
    UpdateEvent,
    ConfirmEvent,
    DeclineEvent,
    DeleteEvent,
    ProfileDetails
}


public class NotificationService
{

    private readonly FirestoreDb firestoreDb;

    public NotificationService()
    {

        var projectId = Environment.GetEnvironmentVariable("GoogleProjectId");
        var googleCredentialsJson = Environment.GetEnvironmentVariable("GOOGLE_CREDENTIALS");

        if (string.IsNullOrEmpty(googleCredentialsJson) || string.IsNullOrEmpty(projectId))
        {
            throw new Exception("Google credentials not found in the environment variable");
        }

        firestoreDb = new FirestoreDbBuilder
        {
            ProjectId = projectId,
            JsonCredentials = googleCredentialsJson
        }.Build();
    }


    public async Task SendEventNotifications(Event ev, Profile currentProfile, UdpateType type, string deviceId)
    {
        //TODO add control over roles
        HashSet<string> profileUserHashes = currentProfile.Users.Select(user => user.Hash).ToHashSet();
        HashSet<string> userHashes = ev.Profiles.SelectMany(profile => profile.Users.Select(user => user.Hash)).Where(hash => !profileUserHashes.Contains(hash)).ToHashSet();

        await Send(userHashes, (type == UdpateType.ConfirmEvent || type == UdpateType.DeleteEvent) ? UdpateType.UpdateEvent : type, ev.Hash, null);
        await Send(profileUserHashes, type, ev.Hash, deviceId);

    }

    private async Task Send(IEnumerable<string> userHashes, UdpateType type, string objectHash, string? deviceId)
    {
        Dictionary<string, object> update = new()
        {
            { "timestamp",  Timestamp.GetCurrentTimestamp()},
            { "type",  type},
            { "hash", objectHash},
            {"deviceId", deviceId ?? ""}
        };


        foreach (string hash in userHashes)
        {
            await firestoreDb.Collection(hash).AddAsync(update);
        }
    }

    public async Task SendMockNotification()
    {
        await Send(["NDjmfkkUXxzINlM47MycQ"], UdpateType.UpdateEvent, "prova", null);
    }

}