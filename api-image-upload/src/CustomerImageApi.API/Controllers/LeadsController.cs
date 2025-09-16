using CustomerImageApi.Application.Interfaces;
using CustomerImageApi.DTOs.Common;
using CustomerImageApi.DTOs.Requests;
using CustomerImageApi.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CustomerImageApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeadsController : ControllerBase
{
    private readonly ILeadService _leadService;

    public LeadsController(ILeadService leadService)
    {
        _leadService = leadService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all leads")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<LeadResponse>>), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<LeadResponse>>>> GetAllLeads()
    {
        try
        {
            var leads = await _leadService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<LeadResponse>>(leads, "Leads retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<IEnumerable<LeadResponse>>(ex.Message));
        }
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get lead by ID")]
    [ProducesResponseType(typeof(ApiResponse<LeadResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<LeadResponse>), 404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<LeadResponse>>> GetLead(int id)
    {
        try
        {
            var lead = await _leadService.GetByIdAsync(id);
            if (lead == null)
                return NotFound(new ApiResponse<LeadResponse>($"Lead with ID {id} not found"));

            return Ok(new ApiResponse<LeadResponse>(lead, "Lead retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<LeadResponse>(ex.Message));
        }
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new lead")]
    [ProducesResponseType(typeof(ApiResponse<LeadResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse<LeadResponse>), 400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<LeadResponse>>> CreateLead([FromBody] CreateLeadRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new ApiResponse<LeadResponse>("Lead name is required"));

            var lead = await _leadService.CreateAsync(request);
            return CreatedAtAction(nameof(GetLead), new { id = lead.Id }, 
                new ApiResponse<LeadResponse>(lead, "Lead created successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<LeadResponse>(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing lead")]
    [ProducesResponseType(typeof(ApiResponse<LeadResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<LeadResponse>), 404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<LeadResponse>>> UpdateLead(int id, [FromBody] CreateLeadRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new ApiResponse<LeadResponse>("Lead name is required"));

            var lead = await _leadService.UpdateAsync(id, request);
            if (lead == null)
                return NotFound(new ApiResponse<LeadResponse>($"Lead with ID {id} not found"));

            return Ok(new ApiResponse<LeadResponse>(lead, "Lead updated successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<LeadResponse>(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a lead")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteLead(int id)
    {
        try
        {
            var result = await _leadService.DeleteAsync(id);
            if (!result)
                return NotFound(new ApiResponse<bool>($"Lead with ID {id} not found"));

            return Ok(new ApiResponse<bool>(true, "Lead deleted successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<bool>(ex.Message));
        }
    }
}