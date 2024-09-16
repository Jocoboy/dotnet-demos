﻿using AspNetCoreDemo.Common;
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

        public BaseInfoController(ISmsInfoService smsInfo, IOprLogService prLog)
        {
            _smsInfo = smsInfo;
            _oprLog = prLog;
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
            //var sign = "XX平台"; // TODO: 数据库获取
            //var msgSgin = $"【{sign}】";
            //var smsinfo = _smsInfo.SendSMSInfo(phone, msgSgin + "验证码：" + code);
            var smsinfo = _smsInfo.SendSMSInfo(phone, "您的验证码是：" + code + "。请不要把验证码泄露给其他人。");
            return ResultHelper<string>.GetResult(ErrorEnum.Success);
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
