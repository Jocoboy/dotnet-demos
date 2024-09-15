using AspNetCoreDemo.Common;
using AspNetCoreDemo.Common.Extensions;
using AspNetCoreDemo.Common.Extensions.JWT;
using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Model.Enums;
using AspNetCoreDemo.Model.ViewModels;
using AspNetCoreDemo.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace AspNetCoreDemo.Web.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class IdentityUserController : ControllerBase
    {
        readonly IPersonService _person;

        public IdentityUserController(IPersonService person)
        {
            _person = person;
        }

        /// <summary>
        /// 前台用户登录
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public MessageDto<string> Login(string key, PersonLoginViewModel model)
        {
            //支持手机账号与密码登录，如果key=null就是手机验证码登录
            if (!ModelState.IsValid)
            {
                return ResultHelper<string>.GetResult(ErrorEnum.DataError, null, ModelStateValidateExtension.GetErrorMessage(ModelState));
            }

            if (key == null) // 手机验证码登录
            {
                if (!MemoryCacheHelper.PhoneCheckCode(model.Phone, model.Code))
                {
                    return ResultHelper<string>.GetResult(ErrorEnum.CodeError);
                }

                return _person.ValPerson(model.Phone);
            }
            else // 密码登录
            {
                if (model.Pwd.Length < 6 || model.Pwd.Length > 20)
                {
                    return ResultHelper<string>.GetResult(ErrorEnum.DataError, message: "密码长度限制6-20位");

                }

                if (!MemoryCacheHelper.CheckCode(key, model.Code))
                {
                    return ResultHelper<string>.GetResult(ErrorEnum.CodeError);
                }

                return _person.ValPerson(model.Phone, model.Pwd);
            }
        }
    }
}
