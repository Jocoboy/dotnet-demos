using AspNetCoreDemo.Common;
using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Service.IService;
using AspNetCoreDemo.Service.Service.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Service.Service
{
    public class SmsInfoService : BaseService<SMSInfo>, ISmsInfoService
    {
        /// <summary>
        /// 短信通道clientId
        /// </summary>
        private string ClientId => "10000253"; // TODO 数据库获取

        /// <summary>
        /// 短信通道地址
        /// </summary>
        //private string MessageUrl => "http://smsc.winupon.com/smsInterfaceTest/sendSms"; // TODO 数据库获取
        private string MessageUrl => "https://106.ihuyi.com/webservice/sms.php?method=Submit"; // TODO 数据库获取

        /// <summary>
        /// 发送短信
        /// </summary>
        public List<SMSInfo> SendSMSInfo(string phone, string smsContent)
        {
            List<SMSInfo> smsinfo = new List<SMSInfo>();
            List<SMSDataDto> list = new List<SMSDataDto>();
            SMSDataDto data = new SMSDataDto()
            {
                SmsId = Guid.NewGuid().ToString("N"),
                SpNumber = "",
                Content = smsContent,
                Phone = phone
            };
            SMSInfo info = new SMSInfo()
            {
                Phone = phone,
                Message = smsContent,
                SendDate = Convert.ToDateTime(DateTime.Now),
                ArrStatus = "",
                Guid = data.SmsId
            };
            smsinfo.Add(info);
            list.Add(data);
            //RegMessageDto<SMSDataDto> dto = new RegMessageDto<SMSDataDto>()
            //{
            //    ClientId = ClientId,
            //    SecurityKey = CommonHelper.GetSecurityKey(ClientId),
            //    SendTime = DateTime.Now.ToString(),
            //    Data = list
            //};

            IHuYiMessageDto dto = new IHuYiMessageDto()
            {
                account = "C83181225",
                password = "7e5881bcf55bafe03262e29906c317f8",
                mobile = phone,
                content = smsContent,
                format = "JSON"
            };

            var httpClient = new HttpClientHelper();

            //var result = httpClient.Post(MessageUrl, JsonConvert.SerializeObject(dto));
            var result = httpClient.Post(MessageUrl, 
                $"account={dto.account}&password={dto.password}&mobile={dto.mobile}&content={dto.content}&format={dto.format}", 
                "application/x-www-form-urlencoded");

            return smsinfo;
        }
    }
}
