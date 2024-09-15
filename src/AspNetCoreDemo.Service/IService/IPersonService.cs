using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Service.IService.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Service.IService
{
    public interface IPersonService: IBaseService<Person>
    {
        MessageDto<string> ValPerson(string phone);
        MessageDto<string> ValPerson(string phone, string pwd);
    }
}
