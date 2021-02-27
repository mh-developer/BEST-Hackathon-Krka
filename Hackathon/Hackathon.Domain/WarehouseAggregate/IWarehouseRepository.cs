using Hackathon.Domain.Shared;
using System;

namespace Hackathon.Domain.WarehouseAggregate
{
    public interface IWarehouseRepository : IRepository<Warehouse, Guid>
    {
    }
}