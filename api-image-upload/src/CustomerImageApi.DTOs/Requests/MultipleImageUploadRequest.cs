using System.ComponentModel.DataAnnotations;

namespace CustomerImageApi.DTOs.Requests;

public class MultipleImageUploadRequest
{
    public List<ImageUploadRequest> Images { get; set; } = new List<ImageUploadRequest>();
}