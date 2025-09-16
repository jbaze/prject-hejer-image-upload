using AutoMapper;
using CustomerImageApi.Application.Interfaces;
using CustomerImageApi.Domain.Entities;
using CustomerImageApi.Domain.Interfaces;
using CustomerImageApi.DTOs.Requests;
using CustomerImageApi.DTOs.Responses;

namespace CustomerImageApi.Application.Services;

public class ImageService : IImageService
{
    private readonly ICustomerImageRepository _customerImageRepository;
    private readonly ILeadImageRepository _leadImageRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILeadRepository _leadRepository;
    private readonly IMapper _mapper;
    private const int MaxImagesPerEntity = 10;

    public ImageService(
        ICustomerImageRepository customerImageRepository,
        ILeadImageRepository leadImageRepository,
        ICustomerRepository customerRepository,
        ILeadRepository leadRepository,
        IMapper mapper)
    {
        _customerImageRepository = customerImageRepository;
        _leadImageRepository = leadImageRepository;
        _customerRepository = customerRepository;
        _leadRepository = leadRepository;
        _mapper = mapper;
    }

    public async Task<ImageListResponse> GetCustomerImagesAsync(int customerId)
    {
        var images = await _customerImageRepository.GetByCustomerIdAsync(customerId);
        var imageResponses = _mapper.Map<List<ImageResponse>>(images);
        
        return new ImageListResponse
        {
            TotalImages = imageResponses.Count,
            RemainingSlots = MaxImagesPerEntity - imageResponses.Count,
            MaxImagesAllowed = MaxImagesPerEntity,
            Images = imageResponses
        };
    }

    public async Task<ImageListResponse> GetLeadImagesAsync(int leadId)
    {
        var images = await _leadImageRepository.GetByLeadIdAsync(leadId);
        var imageResponses = _mapper.Map<List<ImageResponse>>(images);
        
        return new ImageListResponse
        {
            TotalImages = imageResponses.Count,
            RemainingSlots = MaxImagesPerEntity - imageResponses.Count,
            MaxImagesAllowed = MaxImagesPerEntity,
            Images = imageResponses
        };
    }

    public async Task<ImageResponse> UploadCustomerImageAsync(int customerId, ImageUploadRequest request)
    {
        if (!await _customerRepository.ExistsAsync(customerId))
            throw new ArgumentException($"Customer with ID {customerId} not found.");

        var currentImageCount = await _customerImageRepository.GetImageCountByCustomerIdAsync(customerId);
        if (currentImageCount >= MaxImagesPerEntity)
            throw new InvalidOperationException($"Cannot add more than {MaxImagesPerEntity} images per customer.");

        if (string.IsNullOrWhiteSpace(request.Base64Data))
            throw new ArgumentException("Base64 image data is required.");

        var customerImage = _mapper.Map<CustomerImage>(request);
        customerImage.CustomerId = customerId;
        customerImage.UploadedAt = DateTime.UtcNow;

        await _customerImageRepository.CreateAsync(customerImage);
        return _mapper.Map<ImageResponse>(customerImage);
    }

    public async Task<ImageResponse> UploadLeadImageAsync(int leadId, ImageUploadRequest request)
    {
        if (!await _leadRepository.ExistsAsync(leadId))
            throw new ArgumentException($"Lead with ID {leadId} not found.");

        var currentImageCount = await _leadImageRepository.GetImageCountByLeadIdAsync(leadId);
        if (currentImageCount >= MaxImagesPerEntity)
            throw new InvalidOperationException($"Cannot add more than {MaxImagesPerEntity} images per lead.");

        if (string.IsNullOrWhiteSpace(request.Base64Data))
            throw new ArgumentException("Base64 image data is required.");

        var leadImage = _mapper.Map<LeadImage>(request);
        leadImage.LeadId = leadId;
        leadImage.UploadedAt = DateTime.UtcNow;

        await _leadImageRepository.CreateAsync(leadImage);
        return _mapper.Map<ImageResponse>(leadImage);
    }

    public async Task<MultipleImageUploadResponse> UploadMultipleCustomerImagesAsync(int customerId, MultipleImageUploadRequest request)
    {
        if (!await _customerRepository.ExistsAsync(customerId))
            throw new ArgumentException($"Customer with ID {customerId} not found.");

        var response = new MultipleImageUploadResponse
        {
            TotalAttempted = request.Images.Count,
            MaxImagesAllowed = MaxImagesPerEntity
        };

        var currentImageCount = await _customerImageRepository.GetImageCountByCustomerIdAsync(customerId);
        response.RemainingSlots = Math.Max(0, MaxImagesPerEntity - currentImageCount);

        if (response.RemainingSlots == 0)
        {
            response.Errors.Add($"Cannot add any more images. Customer already has {MaxImagesPerEntity} images (maximum allowed).");
            response.FailedUploads = request.Images.Count;
            return response;
        }

        var imagesToProcess = request.Images.Take(response.RemainingSlots).ToList();
        var skippedCount = request.Images.Count - imagesToProcess.Count;

        foreach (var imageRequest in imagesToProcess)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageRequest.Base64Data))
                {
                    response.Errors.Add($"Base64 image data is required for image: {imageRequest.FileName ?? "Unknown"}");
                    response.FailedUploads++;
                    continue;
                }

                var customerImage = _mapper.Map<CustomerImage>(imageRequest);
                customerImage.CustomerId = customerId;
                customerImage.UploadedAt = DateTime.UtcNow;

                await _customerImageRepository.CreateAsync(customerImage);
                response.UploadedImages.Add(_mapper.Map<ImageResponse>(customerImage));
                response.SuccessfulUploads++;
            }
            catch (Exception ex)
            {
                response.Errors.Add($"Failed to upload image {imageRequest.FileName ?? "Unknown"}: {ex.Message}");
                response.FailedUploads++;
            }
        }

        if (skippedCount > 0)
        {
            response.Errors.Add($"{skippedCount} image(s) were skipped due to the {MaxImagesPerEntity}-image limit.");
            response.FailedUploads += skippedCount;
        }

        response.RemainingSlots = Math.Max(0, response.RemainingSlots - response.SuccessfulUploads);

        return response;
    }

    public async Task<MultipleImageUploadResponse> UploadMultipleLeadImagesAsync(int leadId, MultipleImageUploadRequest request)
    {
        if (!await _leadRepository.ExistsAsync(leadId))
            throw new ArgumentException($"Lead with ID {leadId} not found.");

        var response = new MultipleImageUploadResponse
        {
            TotalAttempted = request.Images.Count,
            MaxImagesAllowed = MaxImagesPerEntity
        };

        var currentImageCount = await _leadImageRepository.GetImageCountByLeadIdAsync(leadId);
        response.RemainingSlots = Math.Max(0, MaxImagesPerEntity - currentImageCount);

        if (response.RemainingSlots == 0)
        {
            response.Errors.Add($"Cannot add any more images. Lead already has {MaxImagesPerEntity} images (maximum allowed).");
            response.FailedUploads = request.Images.Count;
            return response;
        }

        var imagesToProcess = request.Images.Take(response.RemainingSlots).ToList();
        var skippedCount = request.Images.Count - imagesToProcess.Count;

        foreach (var imageRequest in imagesToProcess)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageRequest.Base64Data))
                {
                    response.Errors.Add($"Base64 image data is required for image: {imageRequest.FileName ?? "Unknown"}");
                    response.FailedUploads++;
                    continue;
                }

                var leadImage = _mapper.Map<LeadImage>(imageRequest);
                leadImage.LeadId = leadId;
                leadImage.UploadedAt = DateTime.UtcNow;

                await _leadImageRepository.CreateAsync(leadImage);
                response.UploadedImages.Add(_mapper.Map<ImageResponse>(leadImage));
                response.SuccessfulUploads++;
            }
            catch (Exception ex)
            {
                response.Errors.Add($"Failed to upload image {imageRequest.FileName ?? "Unknown"}: {ex.Message}");
                response.FailedUploads++;
            }
        }

        if (skippedCount > 0)
        {
            response.Errors.Add($"{skippedCount} image(s) were skipped due to the {MaxImagesPerEntity}-image limit.");
            response.FailedUploads += skippedCount;
        }

        response.RemainingSlots = Math.Max(0, response.RemainingSlots - response.SuccessfulUploads);

        return response;
    }

    public async Task<bool> DeleteCustomerImageAsync(int customerId, int imageId)
    {
        var image = await _customerImageRepository.GetByIdAsync(imageId);
        if (image == null || image.CustomerId != customerId)
            return false;

        await _customerImageRepository.DeleteAsync(imageId);
        return true;
    }

    public async Task<bool> DeleteLeadImageAsync(int leadId, int imageId)
    {
        var image = await _leadImageRepository.GetByIdAsync(imageId);
        if (image == null || image.LeadId != leadId)
            return false;

        await _leadImageRepository.DeleteAsync(imageId);
        return true;
    }
}