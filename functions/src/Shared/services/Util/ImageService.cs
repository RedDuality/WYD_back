using Azure.Storage.Blobs;

namespace Service;

public class ImageService
{
    public static async Task<string> UploadImageAsync(string eventHash, string imageHash, byte[] imageBytes)
    {
        var connectionString = Environment.GetEnvironmentVariable("BlobConnectionString");
        string containerName = eventHash.ToLower(); // Use event hash as container name
        string blobName = imageHash.ToLower()+".jpg"; // Use image hash as blob name

        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        // Create the container if it doesn't exist
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(blobName);

        using (var ms = new MemoryStream(imageBytes))
        {
            await blobClient.UploadAsync(ms, true);
        }

        return blobClient.Uri.ToString();
    }
}