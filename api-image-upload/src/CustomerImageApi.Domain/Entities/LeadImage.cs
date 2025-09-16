using CustomerImageApi.Domain.Common;

namespace CustomerImageApi.Domain.Entities;

public class LeadImage : BaseEntity
{
    public int LeadId { get; set; }
    public string Base64Data { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public virtual Lead Lead { get; set; } = null!;
}