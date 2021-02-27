using AutoMapper;
using Hackathon.Domain.DeliveryAggregate;

namespace Hackathon.Application.Deliveries
{
    public class DeliveryMapperProfile : Profile
    {
        public DeliveryMapperProfile()
        {
            CreateMap<Delivery, DeliveryDto>();
            CreateMap<DeliveryDto, Delivery>();
        }
    }
}