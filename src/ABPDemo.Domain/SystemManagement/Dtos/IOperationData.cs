using ABPDemo.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPDemo.SystemManagement.Dtos
{
    public interface IOperationData
    {
        public string ToDetail();
        public string ToSummary();
        public string GetOperationName();
        public OperationLogType GetOperationType();

        public static readonly Dictionary<OperationLogType, Type> TypeRegistry = new Dictionary<OperationLogType, Type>();
        static IOperationData()
        {
            TypeRegistry.Add(OperationLogType.Login, typeof(LoginData));
            TypeRegistry.Add(OperationLogType.Logout, typeof(LogoutData));
        }
    }
}
