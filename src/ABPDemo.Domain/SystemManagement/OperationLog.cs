using ABPDemo.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp;
using ABPDemo.SystemManagement.Dtos;
using Newtonsoft.Json;

namespace ABPDemo.SystemManagement
{
    /// <summary>
    /// 操作日志表
    /// </summary>
    public class OperationLog : Entity<Guid>, IHasCreationTime
    {
        /// <summary>
        /// 操作账号显示名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 操作账号用户名
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 账号类型
        /// </summary>
        public UserAccountType AccountType { get; set; }

        /// <summary>
        /// 操作行为
        /// </summary>
        public OperationLogType Operation { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        private IOperationData operationData;
        public void SetOperationData(IOperationData operationData)
        {
            Check.NotNull(operationData, nameof(operationData));

            this.operationData = operationData;
            Data = operationData.ToString();
            Operation = operationData.GetOperationType();
        }
        public IOperationData GetOperationData()
        {
            if (operationData == null && !string.IsNullOrEmpty(Data))
            {
                var type = IOperationData.TypeRegistry[Operation];
                operationData = (IOperationData)JsonConvert.DeserializeObject(Data, type);
            }
            return operationData;
        }

        public static OperationLog Create(string account, string accountName, UserAccountType accountType, IOperationData data)
        {
            var operate = new OperationLog()
            {
                AccountName = accountName,
                Account = account,
                AccountType = accountType
            };
            operate.SetOperationData(data);
            return operate;
        }
    }
}
