using AutoMapper;
using Maintenance.Dto.Auth;
using Maintenance.Dto.Maintenance;
using Maintenance.Entities.Auth;
using Maintenance.Entities.Maintenance;

namespace Maintenance.MapperProfile
{
    public class MaintenanceMapper : Profile
    {
        public MaintenanceMapper()
        {
            CreateMap<FindMaintenanceDto, FindMaintenanceCreterias>();

            CreateMap<FindMaintenanceTechnicalDto, FindMaintenanceTechnicalCreterias>();

            CreateMap<CreateMaintenanceDto, CreateMaintenanceModel>();

            CreateMap<CreateMaintenenceAttachmentDto, CreateMaintenenceAttachmentModel>();

            CreateMap<UpdateMaintenanceStatusDto, UpdateMaintenanceStatusModel>();

            CreateMap<CreateItemRequestDto, CreateItemRequestModel>();

            CreateMap<MaintenenceSparePartViewDto, MaintenenceSparePartViewModel>();

            CreateMap<CreatePurchaseRequestDto, CreatePurchaseRequestModel>();

            CreateMap<CreatePurchaseRequestServiceDto, CreatePurchaseRequestServiceModel>();

            CreateMap<SaveCheckListDto, SaveCheckListModel>();

        }
    }
}
