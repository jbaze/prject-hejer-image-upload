namespace CustomerImageApi.DTOs.Responses;

public class ImageResponse
{
    public int Id { get; set; }
    public string Base64Data { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public DateTime UploadedAt { get; set; }
}