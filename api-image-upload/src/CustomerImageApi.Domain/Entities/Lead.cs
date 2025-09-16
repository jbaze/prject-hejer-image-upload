using CustomerImageApi.Domain.Common;

namespace CustomerImageApi.Domain.Entities;

public class Lead : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Source { get; set; }
    public decimal? Price { get; set; }
    public int? FollowUpDays { get; set; }
    public DateTime? StartingDate { get; set; }
    public string? EstimatedTime { get; set; }

    public virtual ICollection<LeadImage> Images { get; set; } = new List<LeadImage>();
}