using System.ComponentModel.DataAnnotations;

namespace CustomerImageApi.DTOs.Requests;

public class CreateCustomerRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public decimal? Price { get; set; }
    public DateTime? StartingDate { get; set; }
    public string? EstimatedTime { get; set; }
}
