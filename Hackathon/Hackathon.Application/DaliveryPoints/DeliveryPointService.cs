using AutoMapper;
using Hackathon.Domain.DeliveryAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hackathon.Application.DaliveryPoints
{
    public class DeliveryPointService : IDeliveryPointService
    {
        private readonly IDeliveryPointRepository _deliveryPointRepository;
        private readonly IMapper _mapper;

        public DeliveryPointService(
            IDeliveryPointRepository deliveryPointRepository,
            IMapper mapper)
        {
            _deliveryPointRepository = deliveryPointRepository;
            _mapper = mapper;
        }

        public async Task<DeliveryPointDto> CreateAsync(DeliveryPointDto deliveryPointDto)
        {
            if (deliveryPointDto == null)
            {
                throw new ArgumentNullException(nameof(deliveryPointDto));
            }

            var deliveryPoint = _mapper.Map<DeliveryPointDto, DeliveryPoint>(deliveryPointDto);

            _deliveryPointRepository.Add(deliveryPoint);

            await _deliveryPointRepository.SaveChangesAsync();

            return _mapper.Map<DeliveryPoint, DeliveryPointDto>(deliveryPoint);
        }

        public async Task<List<DeliveryPointDto>> GetAllAsync()
        {
            var deliveryPoints = await _deliveryPointRepository.GetAllAsync();

            return _mapper.Map<List<DeliveryPoint>, List<DeliveryPointDto>>(deliveryPoints);
        }

        public async Task<DeliveryPointDto> GetAsync(Guid deliveryPointId)
        {
            if (deliveryPointId == default)
            {
                throw new ArgumentException("Delivery point id is invalid.", nameof(deliveryPointId));
            }

            var deliveryPoint = await _deliveryPointRepository.GetAsync(deliveryPointId);
            if (deliveryPoint == null)
            {
                throw new Exception($"Could not find delivery point with id = '{deliveryPointId}'.");
            }

            return _mapper.Map<DeliveryPoint, DeliveryPointDto>(deliveryPoint);
        }

        public async Task RemoveAsync(Guid deliveryPointId)
        {
            if (deliveryPointId == default)
            {
                throw new ArgumentException("Delivery point id is invalid.", nameof(deliveryPointId));
            }

            var deliveryPoint = await _deliveryPointRepository.GetAsync(deliveryPointId);
            if (deliveryPoint == null)
            {
                throw new Exception($"Could not find delivery point with id = '{deliveryPointId}'.");
            }

            _deliveryPointRepository.Remove(deliveryPoint);

            await _deliveryPointRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(DeliveryPointDto deliveryPointDto)
        {
            if (deliveryPointDto == null)
            {
                throw new ArgumentNullException(nameof(deliveryPointDto));
            }

            if (deliveryPointDto.Id == default)
            {
                throw new ArgumentException("Delivery point id is invalid.", nameof(deliveryPointDto.Id));
            }

            var deliveryPoint = await _deliveryPointRepository.GetAsync(deliveryPointDto.Id);
            if (deliveryPoint == null)
            {
                throw new Exception($"Could not find delivery point with id = '{deliveryPointDto.Id}'.");
            }

            _mapper.Map(deliveryPointDto, deliveryPoint);

            await _deliveryPointRepository.SaveChangesAsync();
        }
    }
}