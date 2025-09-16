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
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all customers")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerResponse>>), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CustomerResponse>>>> GetAllCustomers()
    {
        try
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<CustomerResponse>>(customers, "Customers retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<IEnumerable<CustomerResponse>>(ex.Message));
        }
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get customer by ID")]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), 404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<CustomerResponse>>> GetCustomer(int id)
    {
        try
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound(new ApiResponse<CustomerResponse>($"Customer with ID {id} not found"));

            return Ok(new ApiResponse<CustomerResponse>(customer, "Customer retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<CustomerResponse>(ex.Message));
        }
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new customer")]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), 400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<CustomerResponse>>> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new ApiResponse<CustomerResponse>("Customer name is required"));

            var customer = await _customerService.CreateAsync(request);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, 
                new ApiResponse<CustomerResponse>(customer, "Customer created successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<CustomerResponse>(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing customer")]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), 404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<CustomerResponse>>> UpdateCustomer(int id, [FromBody] CreateCustomerRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new ApiResponse<CustomerResponse>("Customer name is required"));

            var customer = await _customerService.UpdateAsync(id, request);
            if (customer == null)
                return NotFound(new ApiResponse<CustomerResponse>($"Customer with ID {id} not found"));

            return Ok(new ApiResponse<CustomerResponse>(customer, "Customer updated successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<CustomerResponse>(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a customer")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCustomer(int id)
    {
        try
        {
            var result = await _customerService.DeleteAsync(id);
            if (!result)
                return NotFound(new ApiResponse<bool>($"Customer with ID {id} not found"));

            return Ok(new ApiResponse<bool>(true, "Customer deleted successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<bool>(ex.Message));
        }
    }
}