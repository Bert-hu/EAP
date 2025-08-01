using AutoMapper;
using HandlerAgv.Service.Models.Database;
using HandlerAgv.Service.Models.ViewModel;

namespace HandlerAgv.Service.Extension
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<HandlerEquipmentStatus, HandlerEquipmentStatusVm>().ReverseMap();

            //CreateMap<VehicleInfoVM, Pd2AgvInfo>()
            //    .ForMember(destinationMember: des => des.StockerTaskMinPower, memberOptions: opt => { opt.MapFrom(mapExpression: map => map.workThreshold); })
            //    .ForMember(destinationMember: des => des.MachineTaskMinPower, memberOptions: opt => { opt.MapFrom(mapExpression: map => map.urgentChargeThreshold); })
            //    .ReverseMap();

            // 可以继续添加其他映射
        }
    }
}
