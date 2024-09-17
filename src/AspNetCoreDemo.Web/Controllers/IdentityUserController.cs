using AspNetCoreDemo.Common;
using AspNetCoreDemo.Common.Extensions;
using AspNetCoreDemo.Common.Extensions.JWT;
using AspNetCoreDemo.Model.Consts;
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
        readonly IOprLogService _oprLog;

        public IdentityUserController(IPersonService person, IOprLogService oprLog)
        {
            _person = person;
            _oprLog = oprLog;
        }

        /// <summary>
        /// 前台用户注册
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public  MessageDto<string> ParentReg(PersonRegViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ResultHelper<string>.GetResult(ErrorType.DataError, null, ModelStateValidateExtension.GetErrorMessage(ModelState));
            }

            if (_person.GetSingleByExpression(p => p.Phone == model.Phone) != null)
            {
                return ResultHelper<string>.GetResult(ErrorType.RepeatData);
            }

            if (!MemoryCacheHelper.PhoneCheckCode(model.Phone, model.Code))
            {
                return ResultHelper<string>.GetResult(ErrorType.CodeError);
            }

            _oprLog.Add(new OprLog(0, RoleType.person, model.Phone, DateTime.Now, CommonHelper.GetIp(), OprModuleType.register, "手机号为【" + model.Phone + "】的用户注册"));

            _person.Add(new Person()
            {
                Phone = model.Phone,
                Pwd = CommonHelper.GenerateMD5(model.Pwd),
                RegDate = DateTime.Now
            });

            return ResultHelper<string>.GetResult(ErrorType.Success);
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
                return ResultHelper<string>.GetResult(ErrorType.DataError, null, ModelStateValidateExtension.GetErrorMessage(ModelState));
            }

            if (key == null) // 手机验证码登录
            {
                if (!MemoryCacheHelper.PhoneCheckCode(model.Phone, model.Code))
                {
                    return ResultHelper<string>.GetResult(ErrorType.CodeError);
                }

                return _person.ValPerson(model.Phone);
            }
            else // 密码登录
            {
                if (model.Pwd.Length < 6 || model.Pwd.Length > 20)
                {
                    return ResultHelper<string>.GetResult(ErrorType.DataError, message: "密码长度限制6-20位");

                }

                if (!MemoryCacheHelper.CheckCode(key, model.Code))
                {
                    return ResultHelper<string>.GetResult(ErrorType.CodeError);
                }

                return _person.ValPerson(model.Phone, model.Pwd);
            }
        }

        /// <summary>
        /// 前台用户注销账号
        /// </summary>
        [HttpPost]
        public MessageDto<string> ParentLogout()
        {
            var user = _person.GetSingleById(Convert.ToInt32(JWTExtension.GetClaim(JWTExtension.TOKEN_ID)));

            if (user == null)
            {
                return ResultHelper<string>.GetResult(ErrorType.DataError);
            }

            _oprLog.Add(new OprLog(0, RoleType.person, user.Phone, DateTime.Now, CommonHelper.GetIp(), OprModuleType.logout, "手机号为【" + user.Phone + "】的用户注销"));

            _person.Delete(user);

            return ResultHelper<string>.GetResult(ErrorType.Success);
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public MessageDto<string> ResetPassword(ResetPasswordViewModel model)
        {
            if (!MemoryCacheHelper.PhoneCheckCode(model.Phone, model.Code))
            {
                return ResultHelper<string>.GetResult(ErrorType.CodeError);
            }

            if (model.Pwd != model.RePwd)
            {
                return ResultHelper<string>.GetResult(ErrorType.CodeError, message: "两次密码不一致，请重新输入！");
            }

            var user = _person.GetSingleByExpression(x => x.Phone == model.Phone);
            user.Pwd = CommonHelper.GenerateMD5(model.Pwd);
            _person.Update(user);

            return ResultHelper<string>.GetResult(ErrorType.Success);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        [HttpPost]
        public MessageDto<string> ModifyPassword(ModifyPasswordViewModel model)
        {
            var user = _person.GetSingleById(Convert.ToInt32(JWTExtension.GetClaim(JWTExtension.TOKEN_ID)));

            if (CommonHelper.GenerateMD5(model.OgPwd) != user.Pwd)
            {
                return ResultHelper<string>.GetResult(ErrorType.CodeError, message: "密码不正确");
            }

            if (model.NewPwd != model.ReNewPwd)
            {
                return ResultHelper<string>.GetResult(ErrorType.CodeError, message: "两次密码不一致！");
            }

            user.Pwd = CommonHelper.GenerateMD5(model.NewPwd);
            _person.Update(user);

            return ResultHelper<string>.GetResult(ErrorType.Success);
        }
    }
}
