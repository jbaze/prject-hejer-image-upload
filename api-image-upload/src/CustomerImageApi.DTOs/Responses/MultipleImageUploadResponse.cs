namespace CustomerImageApi.DTOs.Responses;

public class MultipleImageUploadResponse
{
    public int SuccessfulUploads { get; set; }
    public int FailedUploads { get; set; }
    public int TotalAttempted { get; set; }
    public List<ImageResponse> UploadedImages { get; set; } = new List<ImageResponse>();
    public List<string> Errors { get; set; } = new List<string>();
    public int RemainingSlots { get; set; }
    public int MaxImagesAllowed { get; set; } = 10;
}