using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore.Entity;
using AutoMapper;

namespace AspNetCoreDemo.Common.AutoMappers
{
    public class UserApiMappersProfile : Profile
    {
        public UserApiMappersProfile()
        {
            CreateMap<SysUser, CurrentUserDto>(MemberList.Source).ReverseMap();
        }
    }
}
