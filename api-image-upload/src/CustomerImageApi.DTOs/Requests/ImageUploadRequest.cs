using System.ComponentModel.DataAnnotations;

namespace CustomerImageApi.DTOs.Requests;

public class ImageUploadRequest
{
    public string Base64Data { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
}