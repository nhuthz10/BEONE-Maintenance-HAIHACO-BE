using AutoMapper;
using Maintenance.Dto.Auth;
using Maintenance.Dto.Maintenance;
using Maintenance.Entities.Auth;
using Maintenance.Entities.Maintenance;
using Maintenance.Entities.Sap;
using System.Data;

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

            CreateMap<CreateGoodReceiptDto, CreateGoodReceiptModel>();

            CreateMap<CreateGoodReceiptDetailDto, CreateGoodReceiptDetailModel>();

            CreateMap<DataRow, TrackingPrDetailViewModel>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(row => row["Type"]))
            .ForMember(dest => dest.Key, opt => opt.MapFrom(row => row["Key"]))
            .ForMember(dest => dest.Process, opt => opt.MapFrom(row => row["Process"]))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(row => row["Value"]))
            .ForMember(dest => dest.DocNo, opt => opt.MapFrom(row => row["DocNo"]))
            .ForMember(dest => dest.User, opt => opt.MapFrom(row => row["User"]))
            .ForMember(dest => dest.Department, opt => opt.MapFrom(row => row["Department"]))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(row => row["Time"]));
        }
    }
}
