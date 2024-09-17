using AspNetCoreDemo.Common.Extensions.Linq;
using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Model.ViewModels;
using AspNetCoreDemo.Repository.IRepository.Base;
using AspNetCoreDemo.Service.IService;
using AspNetCoreDemo.Service.Service.Base;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Service.Service
{
    public class OprLogService : BaseService<OprLog>, IOprLogService
    {
        public IBaseRepository<OprLog> _oprLog;

        public OprLogService(IBaseRepository<OprLog> oprLog)
        {
            _baseRepository = oprLog;
            _oprLog = oprLog;
        }

        public PageResultViewModel<OprLog> GetOprLogPageList(OprLogSearchDto dto)
        {
            var predicate = Expressionable.Create<OprLog>(x => true)
                .AndIF(dto.OprRole, x => x.OprRole == dto.OprRole)
                .AndIF(dto.OprName, x => x.OprName.Contains(dto.OprName))
                .AndIF(dto.OprDateBegin != null, x => x.OprDate >= dto.OprDateBegin)
                .AndIF(dto.OprDateEnd != null, x => x.OprDate <= dto.OprDateEnd)
                .AndIF(dto.OprModule, x => x.OprModule.Contains(dto.OprModule))
                .AndIF(dto.OprRemark, x => x.OprRemark.Contains(dto.OprRemark));

            int totalCount = 0;
            List<OprLog> list = null;

            if (!dto.ExportIsGetAll)
            {
                (list, totalCount) = _oprLog.GetPageListByExpression(predicate, dto);

            }
            else
            {
                list = _oprLog.GetListByExpression(predicate);
                totalCount = list.Count;
            }

            return new PageResultViewModel<OprLog>()
            {
                Count = totalCount,
                List = list
            };
        }
    }
}
