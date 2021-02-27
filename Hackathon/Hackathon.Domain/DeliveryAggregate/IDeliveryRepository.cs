using Hackathon.Domain.Shared;
using System;

namespace Hackathon.Domain.DeliveryAggregate
{
    public interface IDeliveryRepository : IRepository<Delivery, Guid>
    {
    }
}