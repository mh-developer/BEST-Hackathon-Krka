using Hackathon.Domain.Shared;
using System;

namespace Hackathon.Domain.DeliveryAggregate
{
    public interface IDeliveryPointRepository : IRepository<DeliveryPoint, Guid>
    {
    }
}