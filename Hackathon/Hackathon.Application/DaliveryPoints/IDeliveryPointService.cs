using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hackathon.Application.DaliveryPoints
{
    public interface IDeliveryPointService
    {
        Task<List<DeliveryPointDto>> GetAllAsync();

        Task<DeliveryPointDto> GetAsync(Guid deliveryPointId);

        Task<DeliveryPointDto> CreateAsync(DeliveryPointDto deliveryPointDto);

        Task UpdateAsync(DeliveryPointDto deliveryPointDto);

        Task RemoveAsync(Guid deliveryPointId);
    }
}