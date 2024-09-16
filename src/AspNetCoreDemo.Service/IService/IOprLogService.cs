using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Model.ViewModels;
using AspNetCoreDemo.Service.IService.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Service.IService
{
    public interface IOprLogService : IBaseService<OprLog>
    {
        PageResultViewModel<OprLog>  GetOprLogPageList(OprLogSearchDto dto);
    }
}
