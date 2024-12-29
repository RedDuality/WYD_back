
using Azure.Storage.Blobs;
using Dto;
using Model;

namespace Service;

public class BlobService
{

    private static readonly Dictionary<string, string> MimeTypeToExtension = new()
    {
        { "image/jpeg", ".jpg" },
        { "image/png", ".png" },
        { "image/gif", ".gif" },
        { "image/bmp", ".bmp" },
        { "iamge/tiff", ".tiff" },
        /*{ "image/webp", ".webp" },
        { "video/mp4", ".mp4" },
        { "video/x-msvideo", ".avi" },
        { "video/quicktime", ".mov" },
        { "video/x-ms-wmv", ".wmv" },
        { "video/x-flv", ".flv" },
        { "video/x-matroska", ".mkv" }*/
    };

    private static string SanifyBlobData(BlobData blobData)
    {
        if (blobData.Data == null || blobData.Data.Length == 0)
        {
            throw new ArgumentException("Data cannot be null or empty.");
        }

        const int maxSizeInBytes = 20 * 1024 * 1024; // 20MB in bytes
        if (blobData.Data.Length > maxSizeInBytes)
        {
            throw new ArgumentException("Data cannot be larger than 20MB.");
        }
        if (string.IsNullOrWhiteSpace(blobData.MimeType) ||
            !MimeTypeToExtension.ContainsKey(blobData.MimeType.ToLower()))
        {
            throw new ArgumentException($"MIME type '{blobData.MimeType}' is not a valid Blob/video format.");
        }

        return MimeTypeToExtension[blobData.MimeType.ToLower()];
    }

    public static async Task<string> UploadBlobAsync(string parentHash, Blob blob, BlobData blobData)
    {
        var extension = SanifyBlobData(blobData);
        var connectionString = Environment.GetEnvironmentVariable("BlobConnectionString") ?? throw new Exception("BlobConnectionString not found");

        string containerName = parentHash.ToLower(); // Use event hash as container name
        string blobName = blob.Hash.ToLower() + extension; // Use Blob hash as blob name


        var containerClient = new BlobServiceClient(connectionString).GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(publicAccessType: Azure.Storage.Blobs.Models.PublicAccessType.Blob);

        var blobClient = containerClient.GetBlobClient(blobName);

        using (var ms = new MemoryStream(blobData.Data))
        {
            await blobClient.UploadAsync(ms, true);
        }
        return extension;
    }

}