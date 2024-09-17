using AspNetCoreDemo.Common;
using AspNetCoreDemo.Common.Extensions;
using AspNetCoreDemo.Common.Extensions.JWT;
using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Model.Enums;
using AspNetCoreDemo.Model.ViewModels;
using AspNetCoreDemo.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AspNetCoreDemo.Web.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class BaseInfoController : ControllerBase
    {
        readonly ISmsInfoService _smsInfo;
        readonly IOprLogService _oprLog;
        readonly ISysUserService _sysUser;

        public BaseInfoController(ISmsInfoService smsInfo, IOprLogService prLog, ISysUserService sysUser)
        {
            _smsInfo = smsInfo;
            _oprLog = prLog;
            _sysUser = sysUser;
        }

        /// <summary>
        /// 验证码
        /// </summary> 
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValCode(string key)
        {
            return File(MemoryCacheHelper.ValCode(key).ToArray(), @"image/jpeg");
        }

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public MessageDto<string> SendPhoneCode(string phone)
        {
            var code = MemoryCacheHelper.RandValCode(phone);
            //var msgSgin = $"【XX平台】"; // TODO: 数据库获取
            //var smsinfo = _smsInfo.SendSMSInfo(phone, msgSgin + "验证码：" + code);
#if DEBUG
            return ResultHelper<string>.GetResult(ErrorType.Success, code);
#else
            var smsinfo = _smsInfo.SendSMSInfo(phone, "您的验证码是：" + code + "。请不要把验证码泄露给其他人。");
            return ResultHelper<string>.GetResult(ErrorType.Success);
#endif
        }

        /// <summary>
        /// 后台菜单列表
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public MessageDto<List<FirstMenuDto>> GetAdminFrame()
        {
            var user = _sysUser.GetSingleById(Convert.ToInt32(JWTExtension.GetClaim(JWTExtension.TOKEN_ID)));

            var data = _sysUser.GetSysUserMenuByRole(user.RoleCode);

            return ResultHelper<List<FirstMenuDto>>.GetResult(ErrorType.Success, data);
        }

        /// <summary>
        /// 获取日志记录列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public PageResultViewModel<OprLog> GetOprLogPageList(OprLogSearchDto dto)
        {
            var result = _oprLog.GetOprLogPageList(dto);

            return result;
        }
    }
}
