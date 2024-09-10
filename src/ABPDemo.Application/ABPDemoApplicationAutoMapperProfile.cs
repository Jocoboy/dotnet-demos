using ABPDemo.StudentManagement;
using ABPDemo.StudentManagement.Dtos;
using ABPDemo.UserManagement.Dtos;
using AutoMapper;
using System.Linq;
using Volo.Abp.Identity;

namespace ABPDemo;

public class ABPDemoApplicationAutoMapperProfile : Profile
{
    public ABPDemoApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<IdentityUser, AccountDto>();

    }
}
