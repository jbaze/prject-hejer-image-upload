using CustomerImageApi.DTOs.Requests;
using CustomerImageApi.DTOs.Responses;

namespace CustomerImageApi.Application.Interfaces;

public interface IImageService
{
    Task<ImageListResponse> GetCustomerImagesAsync(int customerId);
    Task<ImageListResponse> GetLeadImagesAsync(int leadId);
    Task<ImageResponse> UploadCustomerImageAsync(int customerId, ImageUploadRequest request);
    Task<ImageResponse> UploadLeadImageAsync(int leadId, ImageUploadRequest request);
    Task<MultipleImageUploadResponse> UploadMultipleCustomerImagesAsync(int customerId, MultipleImageUploadRequest request);
    Task<MultipleImageUploadResponse> UploadMultipleLeadImagesAsync(int leadId, MultipleImageUploadRequest request);
    Task<bool> DeleteCustomerImageAsync(int customerId, int imageId);
    Task<bool> DeleteLeadImageAsync(int leadId, int imageId);
}