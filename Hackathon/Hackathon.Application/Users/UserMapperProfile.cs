using AutoMapper;
using Hackathon.Domain.UserAggregate;

namespace Hackathon.Application.Users
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}