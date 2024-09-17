using AspNetCoreDemo.Common.Extensions.JWT;
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
using AspNetCoreDemo.Repository.IRepository.Base;
using AspNetCoreDemo.Common.Extensions;
using AspNetCoreDemo.Model.Consts;
using AspNetCoreDemo.Common.Extensions.Linq;

namespace AspNetCoreDemo.Service.Service
{
    public class SysUserService : BaseService<SysUser>, ISysUserService
    {
        readonly ISysUserRepository _sysUser;
        readonly IBaseRepository<OprLog> _oprLog;
        readonly IBaseRepository<SysRole> _sysRole;
        readonly ISysMenuRepository _sysMenu;
        readonly IMapper _map;

        public SysUserService(IMapper map, ISysUserRepository sysUser, IBaseRepository<OprLog> oprLog, ISysMenuRepository sysMenu, IBaseRepository<SysRole> sysRole)
        {
            _baseRepository = sysUser;
            _map = map;
            _sysUser = sysUser;
            _oprLog = oprLog;
            _sysMenu = sysMenu;
            _sysRole = sysRole;
        }

        public MessageDto<UserLoginResDto> ValSysUser(string userLgnId, string pwd)
        {
            SysUser user = _sysUser.GetSingleByExpression(x => x.UserLgnId == userLgnId);

            if (user == null)
            {
                return ResultHelper<UserLoginResDto>.GetResult(ErrorType.DataError, null, "用户名或密码不正确");
            }

            if (user.IsLock)
            {
                return ResultHelper<UserLoginResDto>.GetResult(ErrorType.DataError, null, "账号已被锁定，请联系管理员");
            }

            if (user.LoginNum >= 5)
            {
                _oprLog.Add(new OprLog()
                {
                    OprId = user.Id,
                    OprRole = user.RoleCode,
                    OprName = user.UserName,
                    OprDate = DateTime.Now,
                    IP = CommonHelper.GetIp(),
                    OprModule = OprModuleType.login,
                    OprRemark = $"管理员【{user.UserName}】失败次数超过5次，账号锁定30分钟",
                });

                user.LoginNum = 0;
                user.LastLoginDate = DateTime.Now;
                _sysUser.Update(user);
                return ResultHelper<UserLoginResDto>.GetResult(ErrorType.DataError, null, "您失败的次数已超过5次，请30分钟后再试");
            }

            if (user.LastLoginDate != null)
            {
                var timeSpan = DateTime.Compare(DateTime.Now, Convert.ToDateTime(user.LastLoginDate).AddMinutes(30));
                if (DateTime.Compare(DateTime.Now, Convert.ToDateTime(user.LastLoginDate).AddMinutes(30)) <= 0)
                {
                    return ResultHelper<UserLoginResDto>.GetResult(ErrorType.DataError, null, "您失败的次数已超过5次暂不可登录，请联系管理员");
                }
            }

            if (user.UserPwd != CommonHelper.GenerateMD5(pwd))
            {
                user.LoginNum += +1;
                _sysUser.Update(user);
                return ResultHelper<UserLoginResDto>.GetResult(ErrorType.DataError, null, "用户名或密码不正确");
            }

            user.LoginNum = 0;
            user.LastLoginDate = null;
            _sysUser.Update(user);

            var dto = _map.Map<CurrentUserDto>(user);
            string token = JWTExtension.GetUserToken(dto);

            _oprLog.Add(new OprLog()
            {
                OprId = user.Id,
                OprRole = user.RoleCode,
                OprName = user.UserName,
                OprDate = DateTime.Now,
                IP = CommonHelper.GetIp(),
                OprModule = OprModuleType.login,
                OprRemark = $"管理员【{user.UserName}】登录"
            });

            return ResultHelper<UserLoginResDto>.GetResult(ErrorType.Success, new UserLoginResDto
            {
                Token = token
            });
        }

        public List<FirstMenuDto> GetSysUserMenuByRole(string role)
        {
            var menus = _sysMenu.GetSysUserMenuByRole(role);

            var result = new List<FirstMenuDto>();
            foreach (var menu in menus)
            {
                var second = new List<SecondMenuDto>();

                if (menu.Code.Substring(2, 2) == "00")
                {
                    var submenus= menus.Where(x => x.Code.Substring(2, 2) != "00" && x.Code.Substring(0, 2) == menu.Code.Substring(0, 2));
                    foreach (var sub in submenus)
                    {
                        second.Add(new SecondMenuDto() { Name = sub.Name, Url = sub.Url});
                    }

                    result.Add(new FirstMenuDto()
                    {
                        Name = menu.Name,
                        Url = menu.Url,
                        Icon = menu.Icon,
                        SecondMenus = second.Count > 0 ? second : null
                    });
                }
            }

            return result;
        }
    }
}
