using Maintenance.Entities.Notification;
using Maintenance.Entities.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.UseCase.NotificationUseCase
{
    public interface INotificationService
    {
        public Task<OperationResult<string>> CreateOrUpdateDeviceToken(CreateOrUpdateDeviceTokenModel model);
        public Task SendNotificationForItemRequestsAsync();
        public Task SendNotificationForPurchaseRequestsAsync();
    }
}
