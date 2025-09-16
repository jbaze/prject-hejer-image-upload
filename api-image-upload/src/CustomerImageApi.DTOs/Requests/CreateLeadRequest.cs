using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerImageApi.DTOs.Requests;

public class CreateLeadRequest
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
}