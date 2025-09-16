namespace CustomerImageApi.DTOs.Responses;

public class CustomerResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public decimal? Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartingDate { get; set; }
    public string? EstimatedTime { get; set; }
    public int ImageCount { get; set; }
}