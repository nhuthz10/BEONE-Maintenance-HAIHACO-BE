using Google.Apis.Auth.OAuth2;
using Maintenance.Entities.Notification;
using Maintenance.Entities.Responses;
using Maintenance.Infrastructure.SqlServer.Data;
using Maintenance.Infrastructure.SqlServer.Repositories.Notification;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.UseCase.NotificationUseCase
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly HttpClient _httpClient;
        private readonly string _projectId = "beone-maintenance-haihaco";
        private readonly GoogleCredential _googleCredential;
        private readonly ApplicationDbContext _dbContext;

        public NotificationService(INotificationRepository notificationRepository, HttpClient httpClient, ApplicationDbContext dbContext)
        {

            _notificationRepository = notificationRepository;
            _httpClient = httpClient;
            _dbContext = dbContext;
            _googleCredential = GoogleCredential.FromFile("service-account.json")
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
        }

        public async Task<OperationResult<string>> CreateOrUpdateDeviceToken(CreateOrUpdateDeviceTokenModel model)
        {
            try
            {
                var result = await _notificationRepository.CreateOrUpdateDeviceToken(model);
                return result;
            }
            catch
            {
                throw;
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var token = await _googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            return token;
        }

        public async Task<NotificationResult> SendNotificationAsync(string deviceToken, string platform, string title, string body, string redirect, Dictionary<string, string>? customData = null)
        {
            var url = $"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send";

            var dataPayload = new Dictionary<string, string>
            {
                { "title", title },
                { "body", body },
                { "click_action", "FLUTTER_NOTIFICATION_CLICK" },
                { "route", redirect }
            };

            if (customData != null)
            {
                foreach (var kv in customData)
                {
                    dataPayload[kv.Key] = kv.Value;
                }
            }

            var message = new
            {
                message = new
                {
                    token = deviceToken,

                    notification = platform == "IOS" ? new
                    {
                        title,
                        body
                    } : null,

                    data = dataPayload,

                    android = new
                    {
                        priority = "high",
                    },

                    apns = new
                    {
                        payload = new
                        {
                            aps = new
                            {
                                sound = "default"
                            }
                        }
                    }
                }
            };

            var jsonMessage = JsonConvert.SerializeObject(message);
            var accessToken = await GetAccessTokenAsync();

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            return new NotificationResult
            {
                Success = response.IsSuccessStatusCode,
                Response = responseContent
            };
        }

        public async Task SendNotificationForItemRequestsAsync()
        {
            try
            {
                var logs = await _dbContext.NotificationLogs.Where(d => d.RefType == "ItemRequest" && d.RetryCount < 3 && (d.Status == 0 || d.Status == 2))
                    .OrderBy(x => x.CreatedAt)
                    .Take(100).ToListAsync();

                if (!logs.Any()) return;

                foreach (var log in logs)
                {
                    log.Status = -1;
                }

                await _dbContext.SaveChangesAsync();

                foreach (var log in logs)
                {
                    try
                    {
                        Dictionary<string, string>?
                            data = null;

                        if (!string.IsNullOrWhiteSpace(log.DataJson))
                        {
                            data = JsonConvert.DeserializeObject<Dictionary<string, string>>(log.DataJson);
                        }

                        var result = await SendNotificationAsync(log.FcmToken, log.Platform ?? "", log.Title, log.Body, $"/home-technical/maintenance/{log.DocEntry}", data);

                        if (result.Success)
                        {
                            log.Status = 1;
                            log.SentAt = DateTime.Now;
                            log.ErrorMessage = null;
                        }
                        else
                        {
                            log.Status = 2;
                            log.RetryCount++;
                            log.ErrorMessage = result.Response;

                            if (result.Response.Contains("UNREGISTERED") || result.Response.Contains("INVALID_ARGUMENT") || result.Response.Contains("registration-token-not-registered"))
                            {
                                var device = await _dbContext.UserDevices.FirstOrDefaultAsync(x => x.DeviceId == log.DeviceId);

                                if (device != null)
                                {
                                    device.IsActive = false;
                                    device.UpdatedAt = DateTime.Now;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Status = 2;
                        log.RetryCount++;
                        log.ErrorMessage = ex.Message;
                    }
                }

                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task SendNotificationForPurchaseRequestsAsync()
        {
            try
            {
                var logs = await _dbContext.NotificationLogs.Where(d => d.RefType == "PurchaseRequest" && d.RetryCount < 3 && (d.Status == 0 || d.Status == 2))
                    .OrderBy(x => x.CreatedAt)
                    .Take(100).ToListAsync();

                if (!logs.Any()) return;

                foreach (var log in logs)
                {
                    log.Status = -1;
                }

                await _dbContext.SaveChangesAsync();

                foreach (var log in logs)
                {
                    try
                    {
                        Dictionary<string, string>?
                            data = null;

                        if (!string.IsNullOrWhiteSpace(log.DataJson))
                        {
                            data = JsonConvert.DeserializeObject<Dictionary<string, string>>(log.DataJson);
                        }

                        var result = await SendNotificationAsync(log.FcmToken, log.Platform ?? "", log.Title, log.Body, $"/home-technical/maintenance/{log.DocEntry}", data);

                        if (result.Success)
                        {
                            log.Status = 1;
                            log.SentAt = DateTime.Now;
                            log.ErrorMessage = null;
                        }
                        else
                        {
                            log.Status = 2;
                            log.RetryCount++;
                            log.ErrorMessage = result.Response;

                            if (result.Response.Contains("UNREGISTERED") || result.Response.Contains("INVALID_ARGUMENT") || result.Response.Contains("registration-token-not-registered"))
                            {
                                var device = await _dbContext.UserDevices.FirstOrDefaultAsync(x => x.DeviceId == log.DeviceId);

                                if (device != null)
                                {
                                    device.IsActive = false;
                                    device.UpdatedAt = DateTime.Now;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Status = 2;
                        log.RetryCount++;
                        log.ErrorMessage = ex.Message;
                    }
                }

                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
