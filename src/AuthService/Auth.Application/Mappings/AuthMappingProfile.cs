using Auth.Application.Commands;
using Auth.Domain.Dtos;
using Auth.Domain.Entities;
using AutoMapper;

namespace Auth.Application.Mappings
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            CreateMap<RegisterAdminCommand, AppUser>();
            CreateMap<LoginCommand, AppUser>();
            CreateMap<AppUser, AppUserDto>();

            CreateMap<UpdateAppUserCommand, AppUser>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateProfileCommand, AppUser>()
               .ForMember(dest => dest.Id, opt => opt.Ignore());

        }
    }
}
