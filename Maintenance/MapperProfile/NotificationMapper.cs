using AutoMapper;
using Maintenance.Dto.Notification;
using Maintenance.Entities.Notification;

namespace Maintenance.MapperProfile
{
    public class NotificationMapper : Profile
    {
        public NotificationMapper()
        {
            CreateMap<CreateOrUpdateDeviceTokenDto, CreateOrUpdateDeviceTokenModel>();
        }
    }
}
