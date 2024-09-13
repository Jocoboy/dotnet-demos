using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Common.AutoMappers
{
    public static class UserApiMappers
    {
        static UserApiMappers()
        {
            var config = new MapperConfiguration(m =>
            {
                m.AddProfile<UserApiMappersProfile>();
            });
        }
    }
}
