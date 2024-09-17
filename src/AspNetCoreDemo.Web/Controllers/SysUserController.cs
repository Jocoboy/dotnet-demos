using AspNetCoreDemo.Common.Extensions.JWT;
using AspNetCoreDemo.Common.Extensions;
using AspNetCoreDemo.Common;
using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Model.Enums;
using AspNetCoreDemo.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreDemo.Service.IService;
using AutoMapper;

namespace AspNetCoreDemo.Web.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class SysUserController : ControllerBase
    {

        readonly ISysUserService _sysUser;

        public SysUserController(ISysUserService sysUser, IMapper map)
        {
            _sysUser = sysUser;
        }

        /// <summary>
        /// 后台用户登录
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public MessageDto<UserLoginResDto> Login(string key, UserLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ResultHelper<UserLoginResDto>.GetResult(ErrorType.DataError, null, ModelStateValidateExtension.GetErrorMessage(ModelState));
            }

            if (!MemoryCacheHelper.CheckCode(key, model.Code))
            {
                return ResultHelper<UserLoginResDto>.GetResult(ErrorType.CodeError, null, EnumExtension.GetRemark(ErrorType.CodeError));
            }

            return _sysUser.ValSysUser(model.UserLgnId, model.UserPwd);
        }
    }
}
