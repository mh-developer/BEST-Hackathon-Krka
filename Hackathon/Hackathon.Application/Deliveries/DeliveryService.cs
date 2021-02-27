using AutoMapper;
using Hackathon.Domain.DeliveryAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hackathon.Application.Deliveries
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IMapper _mapper;

        public DeliveryService(
            IDeliveryRepository deliveryRepository,
            IMapper mapper)
        {
            _deliveryRepository = deliveryRepository;
            _mapper = mapper;
        }

        public async Task<DeliveryDto> CreateAsync(DeliveryDto deliveryDto)
        {
            if (deliveryDto == null)
            {
                throw new ArgumentNullException(nameof(deliveryDto));
            }

            var delivery = _mapper.Map<DeliveryDto, Delivery>(deliveryDto);

            _deliveryRepository.Add(delivery);

            await _deliveryRepository.SaveChangesAsync();

            return _mapper.Map<Delivery, DeliveryDto>(delivery);
        }

        public async Task<List<DeliveryDto>> GetAllAsync()
        {
            var deliveries = await _deliveryRepository.GetAllAsync();

            return _mapper.Map<List<Delivery>, List<DeliveryDto>>(deliveries);
        }

        public async Task<DeliveryDto> GetAsync(Guid deliveryId)
        {
            if (deliveryId == default)
            {
                throw new ArgumentException("Delivery id is invalid.", nameof(deliveryId));
            }

            var delivery = await _deliveryRepository.GetAsync(deliveryId);
            if (delivery == null)
            {
                throw new Exception($"Could not find delivery with id = '{deliveryId}'.");
            }

            return _mapper.Map<Delivery, DeliveryDto>(delivery);
        }

        public async Task RemoveAsync(Guid deliveryId)
        {
            if (deliveryId == default)
            {
                throw new ArgumentException("Delivery id is invalid.", nameof(deliveryId));
            }

            var delivery = await _deliveryRepository.GetAsync(deliveryId);
            if (delivery == null)
            {
                throw new Exception($"Could not find delivery with id = '{deliveryId}'.");
            }

            _deliveryRepository.Remove(delivery);

            await _deliveryRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(DeliveryDto deliveryDto)
        {
            if (deliveryDto == null)
            {
                throw new ArgumentNullException(nameof(deliveryDto));
            }

            if (deliveryDto.Id == default)
            {
                throw new ArgumentException("Delivery id is invalid.", nameof(deliveryDto.Id));
            }

            var delivery = await _deliveryRepository.GetAsync(deliveryDto.Id);
            if (delivery == null)
            {
                throw new Exception($"Could not find delivery with id = '{deliveryDto.Id}'.");
            }

            _mapper.Map(deliveryDto, delivery);

            await _deliveryRepository.SaveChangesAsync();
        }
    }
}