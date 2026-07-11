using Hangfire;
using Maintenance.Infrastructure.SqlServer.Repositories.Notification;
using Maintenance.UseCase.NotificationUseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.UseCase.NotificationJobUseCase
{
    public class NotificationJobService
    {
        private readonly INotificationService _notificationService;

        public NotificationJobService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void CreateSendNotificationForItemRequestsJob()
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var options = new RecurringJobOptions
            {
                TimeZone = timeZone
            };
            RecurringJob.AddOrUpdate("send-noti-item-request",
                methodCall: () => SendNotificationForItemRequests(),
                 "*/1 * * * *", options: options);
        }

        public void CreateSendNotificationForPurchaseRequestsJob()
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var options = new RecurringJobOptions
            {
                TimeZone = timeZone
            };
            RecurringJob.AddOrUpdate("send-noti-purchase-request",
                methodCall: () => SendNotificationForPurchaseRequests(),
                 "*/1 * * * *", options: options);
        }

        public async Task SendNotificationForItemRequests()
        {
            try
            {
                await _notificationService.SendNotificationForItemRequestsAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task SendNotificationForPurchaseRequests()
        {
            try
            {
                await _notificationService.SendNotificationForPurchaseRequestsAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
