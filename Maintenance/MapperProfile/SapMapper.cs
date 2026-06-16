using AutoMapper;
using Maintenance.Dto.Auth;
using Maintenance.Entities.Auth;
using Maintenance.Entities.Sap;
using System.Data;

namespace Maintenance.MapperProfile
{
    public class SapMapper : Profile
    {
        public SapMapper()
        {
            CreateMap<DataRow, WarehouseViewModel>()
            .ForMember(dest => dest.WhsCode, opt => opt.MapFrom(row => row["WhsCode"]))
            .ForMember(dest => dest.IsWms, opt => opt.MapFrom(row => row["IsWms"]));

            CreateMap<DataRow, DimensionViewModel>()
            .ForMember(dest => dest.Dimension1, opt => opt.MapFrom(row => row["Dimension1"]))
            .ForMember(dest => dest.Dimension2, opt => opt.MapFrom(row => row["Dimension2"]))
            .ForMember(dest => dest.Dimension3, opt => opt.MapFrom(row => row["Dimension3"]))
            .ForMember(dest => dest.Dimension4, opt => opt.MapFrom(row => row["Dimension4"]))
            .ForMember(dest => dest.Dimension5, opt => opt.MapFrom(row => row["Dimension5"]));
        }
    }
}
