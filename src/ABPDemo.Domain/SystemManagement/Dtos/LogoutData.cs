using ABPDemo.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPDemo.SystemManagement.Dtos
{
    public class LogoutData : IOperationData
    {
        public string IP { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        public string ToDetail()
        {
            return $"登出IP:{IP}";
        }
        public string ToSummary() => ToDetail();
        public string GetOperationName() => "登出";
        public OperationLogType GetOperationType() => OperationLogType.Logout;
    }
}
