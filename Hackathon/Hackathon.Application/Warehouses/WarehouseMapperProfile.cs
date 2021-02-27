using AutoMapper;
using Hackathon.Domain.WarehouseAggregate;

namespace Hackathon.Application.Warehouses
{
    public class WarehouseMapperProfile : Profile
    {
        public WarehouseMapperProfile()
        {
            CreateMap<Warehouse, WarehouseDto>();
            CreateMap<WarehouseDto, Warehouse>();
        }
    }
}