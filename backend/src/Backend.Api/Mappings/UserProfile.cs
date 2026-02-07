using AutoMapper;
using Backend.Api.Domain.Entities;
using Backend.Api.Dtos;

namespace Backend.Api.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
    }
}
