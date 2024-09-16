using AspNetCoreDemo.Common.Extensions.JWT;
using AspNetCoreDemo.Common;
using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Model.Enums;
using AspNetCoreDemo.Repository.IRepository.Base;
using AspNetCoreDemo.Service.IService;
using AspNetCoreDemo.Service.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using AspNetCoreDemo.Common.Extensions;

namespace AspNetCoreDemo.Service.Service
{
    public class PersonService : BaseService<Person>, IPersonService
    {
        readonly IBaseRepository<Person> _person;
        readonly IBaseRepository<OprLog> _oprLog;

        public PersonService(IBaseRepository<Person> person, IBaseRepository<OprLog> oprLog)
        {
            _person = person;
            _oprLog = oprLog;
        }

        public MessageDto<string> ValPerson(string phone)
        {
            var person = _person.GetSingleByExpression(x => x.Phone == phone);

            if (person == null)
            {
                return ResultHelper<string>.GetResult(ErrorEnum.DataError, null, "手机号不正确");
            }

            person.LastLoginDate = DateTime.Now;
            _person.Update(person);

            _oprLog.Add(new OprLog()
            {
                OprId = person.Id,
                OprRole = "person",
                OprName = person.Phone,
                OprDate = DateTime.Now,
                IP = CommonHelper.GetIp(),
                OprModule = EnumExtension.GetRemark(OprModuleType.Login),
                OprRemark = "用户【" + person.Phone + "】登录",
            });

            var token = JWTExtension.GetUserToken(new CurrentUserDto()
            {
                Id = person.Id,
                UserLgnId = person.Phone,
                RoleCode = "person",
                UserName = ""
            });

            return ResultHelper<string>.GetResult(ErrorEnum.Success, data: token);
        }

        public MessageDto<string> ValPerson(string phone, string pwd)
        {
            var person = _person.GetSingleByExpression(x => x.Phone == phone);

            if (person == null)
            {
                return ResultHelper<string>.GetResult(ErrorEnum.DataError, null, "手机号不正确");
            }

            if (person.LoginNum == 5)//错误登录大于等于5次
            {
                person.LoginNum = 0;
                person.LastLoginDate = DateTime.Now;
                _person.Update(person);
                return ResultHelper<string>.GetResult(ErrorEnum.DataError, message: "已登录错误5次，账号已锁定，请15分钟后再进行登录");
            }

            if (DateTime.Compare(DateTime.Now, Convert.ToDateTime(person.LastLoginDate).AddMinutes(15)) <= 0)//解锁时间
            {
                return ResultHelper<string>.GetResult(ErrorEnum.DataError, message: "已登录错误5次，账号已锁定，请15分钟后再进行登录");
            }

            if (person.Pwd != CommonHelper.GenerateMD5(pwd))
            {
                person.LoginNum += 1;
                _person.Update(person);
                return ResultHelper<string>.GetResult(ErrorEnum.DataError, message: "账号密码错误");
            }

            person.LoginNum = 0;
            _person.Update(person);

            _oprLog.Add(new OprLog()
            {
                OprId = person.Id,
                OprRole = "person",
                OprName = person.Phone,
                OprDate = DateTime.Now,
                IP = CommonHelper.GetIp(),
                OprModule = EnumExtension.GetRemark(OprModuleType.Login),
                OprRemark = "用户【" + person.Phone + "】登录",
            });

            var token = JWTExtension.GetUserToken(new CurrentUserDto()
            {
                Id = person.Id,
                UserLgnId = person.Phone,
                RoleCode = "person",
                UserName = ""
            });

            return ResultHelper<string>.GetResult(ErrorEnum.Success, data: token);
        }
    }
}
