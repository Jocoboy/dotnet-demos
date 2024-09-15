﻿using AspNetCoreDemo.Common.Extensions.JWT;
using AspNetCoreDemo.Common;
using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Model.Enums;
using AspNetCoreDemo.Repository.IRepository;
using AspNetCoreDemo.Repository.Repository.Base;
using AspNetCoreDemo.Service.IService;
using AspNetCoreDemo.Service.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace AspNetCoreDemo.Service.Service
{
    public class SysUserService : BaseService<SysUser>, ISysUserService
    {
        readonly ISysUserRepository _sysUser;
        readonly IMapper _map;

        public SysUserService(IMapper map, ISysUserRepository sysUser)
        {
            _map = map;
            _sysUser = sysUser;
        }

        public MessageDto<UserLoginResDto> ValSysUser(string userLgnId, string pwd)
        {
            SysUser user = _sysUser.GetSingleByExpression(x => x.UserLgnId == userLgnId);

            if (user == null)
            {
                return ResultHelper<UserLoginResDto>.GetResult(ErrorEnum.DataError, null, "用户名或密码不正确");
            }

            if (user.IsLock)
            {
                return ResultHelper<UserLoginResDto>.GetResult(ErrorEnum.DataError, null, "账号已被锁定，请联系管理员");
            }

            if (user.LoginNum >= 5)
            {
                user.LoginNum = 0;
                user.LastLoginDate = DateTime.Now;
                _sysUser.Update(user);
                return ResultHelper<UserLoginResDto>.GetResult(ErrorEnum.DataError, null, "您失败的次数已超过5次，请30分钟后再试");
            }

            if (user.LastLoginDate != null)
            {
                var timeSpan = DateTime.Compare(DateTime.Now, Convert.ToDateTime(user.LastLoginDate).AddMinutes(30));
                if (DateTime.Compare(DateTime.Now, Convert.ToDateTime(user.LastLoginDate).AddMinutes(30)) <= 0)
                {
                    return ResultHelper<UserLoginResDto>.GetResult(ErrorEnum.DataError, null, "您失败的次数已超过5次暂不可登录，请联系管理员");
                }
            }

            if (user.UserPwd != CommonHelper.GenerateMD5(pwd))
            {
                user.LoginNum += +1;
                _sysUser.Update(user);
                return ResultHelper<UserLoginResDto>.GetResult(ErrorEnum.DataError, null, "用户名或密码不正确");
            }

            user.LoginNum = 0;
            user.LastLoginDate = null;
            _sysUser.Update(user);

            var dto = _map.Map<CurrentUserDto>(user);
            string token = JWTExtension.GetUserToken(dto);

            return ResultHelper<UserLoginResDto>.GetResult(ErrorEnum.Success, new UserLoginResDto
            {
                Token = token
            });
        }
    }
}
