
using Google.Cloud.Firestore;

namespace Service;

public enum UdpateType
{
    Event,
    Profile,
    Confirm
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

    public async Task SendUpdateNotifications(List<string> userHashes, UdpateType type, int id)
    {
        Dictionary<string, object> update = new()
        {
            { "timestamp",  Timestamp.GetCurrentTimestamp()},
            { "type",  type},
            { "id", id}
        };


        foreach (string hash in userHashes)
        {
            await firestoreDb.Collection(hash).AddAsync(update);
        }

    }



}