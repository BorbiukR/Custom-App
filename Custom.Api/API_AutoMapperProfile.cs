using AutoMapper;
using Custom.Api.Models.Request;
using Custom.BL.Models;

namespace Custom.Api
{
    public class API_AutoMapperProfile : Profile
    {
        public API_AutoMapperProfile()
        {
            CreateMap<CustomsBusRequest, CustomsDataDTO>();
            CreateMap<CustomsBikeRequest, CustomsDataDTO>();
            CreateMap<CustomsCarRequest, CustomsDataDTO>();
            CreateMap<CustomsElectricCarRequest, CustomsDataDTO>();
            CreateMap<CustomsTruckRequest, CustomsDataDTO>();
        }     
    }
}