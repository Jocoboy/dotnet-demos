using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Service.IService.Base;

namespace AspNetCoreDemo.Service.IService
{
    public interface ISmsInfoService : IBaseService<SMSInfo>
    {
        List<SMSInfo> SendSMSInfo(string phone, string smsContent);
    }
}
