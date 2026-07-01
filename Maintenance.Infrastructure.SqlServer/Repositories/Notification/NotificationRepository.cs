using AutoMapper;
using Maintenance.Entities.Notification;
using Maintenance.Entities.Responses;
using Maintenance.Infrastructure.SqlServer.Data;
using Maintenance.Infrastructure.SqlServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.Repositories.Notification
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationRepository(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<OperationResult<string>> CreateOrUpdateDeviceToken(CreateOrUpdateDeviceTokenModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.AccountId);

                if (user == null)
                {
                    return OperationResult<string>.Fail(ErrorCode.NotFound, "Cannot find user");
                }

                var activeDevices = await _dbContext.UserDevices
                    .Where(x => x.DeviceId == model.DeviceId
                             && x.AccountId != model.AccountId
                             && x.IsActive)
                    .ToListAsync();

                foreach (var device in activeDevices)
                {
                    device.IsActive = false;
                    device.UpdatedAt = DateTime.Now;
                }

                var existingToken = await _dbContext.UserDevices.FirstOrDefaultAsync(x => x.AccountId == model.AccountId && x.DeviceId == model.DeviceId);

                if (existingToken != null)
                {
                    existingToken.FcmToken = model.Token;
                    existingToken.Platform = model.Platform;
                    existingToken.UpdatedAt = DateTime.Now;
                    existingToken.IsActive = true;
                }
                else
                {
                    _dbContext.UserDevices.Add(new UserDevices
                    {
                        FcmToken = model.Token,
                        AccountId = model.AccountId,
                        Platform = model.Platform,
                        DeviceId = model.DeviceId,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    });
                }

                await _dbContext.SaveChangesAsync();

                return OperationResult<string>.Success("Create or update device token successfully");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
