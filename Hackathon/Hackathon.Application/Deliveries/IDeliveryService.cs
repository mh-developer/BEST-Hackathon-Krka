using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hackathon.Application.Deliveries
{
    public interface IDeliveryService
    {
        Task<List<DeliveryDto>> GetAllAsync();

        Task<DeliveryDto> GetAsync(Guid deliveryId);

        Task<DeliveryDto> CreateAsync(DeliveryDto deliveryDto);

        Task UpdateAsync(DeliveryDto deliveryDto);

        Task RemoveAsync(Guid deliveryId);
    }
}