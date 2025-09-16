namespace CustomerImageApi.DTOs.Responses;

public class ImageListResponse
{
    public int TotalImages { get; set; }
    public int RemainingSlots { get; set; }
    public int MaxImagesAllowed { get; set; } = 10;
    public List<ImageResponse> Images { get; set; } = new List<ImageResponse>();
}
