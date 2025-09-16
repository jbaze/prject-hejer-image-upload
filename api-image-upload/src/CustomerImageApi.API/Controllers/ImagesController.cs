using CustomerImageApi.Application.Interfaces;
using CustomerImageApi.DTOs.Common;
using CustomerImageApi.DTOs.Requests;
using CustomerImageApi.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CustomerImageApi.API.Controllers;

[ApiController]
[Route("api/customers/{customerId}/[controller]")]
[Authorize]
public class ImagesController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImagesController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all images for a customer")]
    [ProducesResponseType(typeof(ApiResponse<ImageListResponse>), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<ImageListResponse>>> GetCustomerImages(int customerId)
    {
        try
        {
            var images = await _imageService.GetCustomerImagesAsync(customerId);
            return Ok(new ApiResponse<ImageListResponse>(images, "Customer images retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ImageListResponse>(ex.Message));
        }
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Upload an image for a customer")]
    [ProducesResponseType(typeof(ApiResponse<ImageResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse<ImageResponse>), 400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<ImageResponse>>> UploadCustomerImage(int customerId, [FromBody] ImageUploadRequest request)
    {
        try
        {
            var image = await _imageService.UploadCustomerImageAsync(customerId, request);
            return CreatedAtAction(nameof(GetCustomerImages), new { customerId }, 
                new ApiResponse<ImageResponse>(image, "Image uploaded successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<ImageResponse>(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<ImageResponse>(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ImageResponse>(ex.Message));
        }
    }

    [HttpPost("bulk")]
    [SwaggerOperation(Summary = "Upload multiple images for a customer (up to the 10-image limit)")]
    [ProducesResponseType(typeof(ApiResponse<MultipleImageUploadResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<MultipleImageUploadResponse>), 400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<MultipleImageUploadResponse>>> UploadMultipleCustomerImages(int customerId, [FromBody] MultipleImageUploadRequest request)
    {
        try
        {
            if (request.Images == null || !request.Images.Any())
                return BadRequest(new ApiResponse<MultipleImageUploadResponse>("At least one image is required"));

            var result = await _imageService.UploadMultipleCustomerImagesAsync(customerId, request);
            
            // Return 200 OK even if some uploads failed, as this is a bulk operation
            var message = result.SuccessfulUploads == result.TotalAttempted 
                ? "All images uploaded successfully" 
                : $"{result.SuccessfulUploads} of {result.TotalAttempted} images uploaded successfully";
                
            return Ok(new ApiResponse<MultipleImageUploadResponse>(result, message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<MultipleImageUploadResponse>(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<MultipleImageUploadResponse>(ex.Message));
        }
    }

    [HttpDelete("{imageId}")]
    [SwaggerOperation(Summary = "Delete an image from a customer")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCustomerImage(int customerId, int imageId)
    {
        try
        {
            var result = await _imageService.DeleteCustomerImageAsync(customerId, imageId);
            if (!result)
                return NotFound(new ApiResponse<bool>($"Image with ID {imageId} not found for customer {customerId}"));

            return Ok(new ApiResponse<bool>(true, "Image deleted successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<bool>(ex.Message));
        }
    }
}