using CustomerImageApi.Domain.Common;

namespace CustomerImageApi.Domain.Entities;

public class CustomerImage : BaseEntity
{
    public int CustomerId { get; set; }
    public string Base64Data { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public virtual Customer Customer { get; set; } = null!;
}