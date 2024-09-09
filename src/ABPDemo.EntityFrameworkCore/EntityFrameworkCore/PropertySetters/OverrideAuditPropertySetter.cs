using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;
using Volo.Abp.Users;

namespace ABPDemo.EntityFrameworkCore.PropertySetters
{
    public class OverrideAuditPropertySetter : AuditPropertySetter
    {
        public OverrideAuditPropertySetter(ICurrentUser currentUser, ICurrentTenant currentTenant, IClock clock) : base(currentUser, currentTenant, clock)
        {
        }

        public override void SetCreationProperties(object targetObject)
        {
            base.SetCreationProperties(targetObject);
            SetLastModificationTime(targetObject);
        }
    }
}
