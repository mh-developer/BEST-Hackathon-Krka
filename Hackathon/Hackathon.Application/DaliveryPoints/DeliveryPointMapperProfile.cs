using AutoMapper;
using Hackathon.Domain.DeliveryAggregate;

namespace Hackathon.Application.DaliveryPoints
{
    public class DeliveryPointMapperProfile : Profile
    {
        public DeliveryPointMapperProfile()
        {
            CreateMap<DeliveryPoint, DeliveryPointDto>();
            CreateMap<DeliveryPointDto, DeliveryPoint>();
        }
    }
}