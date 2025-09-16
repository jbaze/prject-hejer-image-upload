using AutoMapper;
using CustomerImageApi.Domain.Entities;
using CustomerImageApi.DTOs.Responses;

namespace CustomerImageApi.Application.Mapping;

public class AuthenticationMappingProfile : Profile
{
    public AuthenticationMappingProfile()
    {
        CreateMap<User, UserInfo>();
    }
}