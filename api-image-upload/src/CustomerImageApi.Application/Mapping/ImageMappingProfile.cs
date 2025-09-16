using AutoMapper;
using CustomerImageApi.Domain.Entities;
using CustomerImageApi.DTOs.Requests;
using CustomerImageApi.DTOs.Responses;

namespace CustomerImageApi.Application.Mapping;

public class ImageMappingProfile : Profile
{
    public ImageMappingProfile()
    {
        CreateMap<CustomerImage, ImageResponse>();
        
        CreateMap<ImageUploadRequest, CustomerImage>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore());

        CreateMap<LeadImage, ImageResponse>();
        
        CreateMap<ImageUploadRequest, LeadImage>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.LeadId, opt => opt.Ignore())
            .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Lead, opt => opt.Ignore());
    }
}