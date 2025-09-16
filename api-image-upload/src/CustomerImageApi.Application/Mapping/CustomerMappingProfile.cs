using AutoMapper;
using CustomerImageApi.Domain.Entities;
using CustomerImageApi.DTOs.Requests;
using CustomerImageApi.DTOs.Responses;

namespace CustomerImageApi.Application.Mapping;

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        CreateMap<Customer, CustomerResponse>()
            .ForMember(dest => dest.ImageCount, opt => opt.MapFrom(src => src.Images != null ? src.Images.Count : 0));
        
        CreateMap<CreateCustomerRequest, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore());

        CreateMap<CreateCustomerRequest, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}