using Maintenance.Entities.Notification;
using Maintenance.Entities.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.Repositories.Notification
{
    public interface INotificationRepository
    {
        public Task<OperationResult<string>> CreateOrUpdateDeviceToken(CreateOrUpdateDeviceTokenModel model);
    }
}
