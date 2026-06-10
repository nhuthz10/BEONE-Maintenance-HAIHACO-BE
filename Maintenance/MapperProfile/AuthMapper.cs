using AutoMapper;
using Maintenance.Dto.Auth;
using Maintenance.Entities.Auth;

namespace Maintenance.MapperProfile
{
    public class AuthMapper : Profile
    {
        public AuthMapper()
        {
            CreateMap<LoginDto, LoginModel>();
            CreateMap<LoginAdminDto, LoginAdminModel>();
        }
    }
}
