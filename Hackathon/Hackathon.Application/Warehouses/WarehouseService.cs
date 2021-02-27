using AutoMapper;
using Hackathon.Domain.WarehouseAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hackathon.Application.Warehouses
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IMapper _mapper;

        public WarehouseService(
            IWarehouseRepository warehouseRepository,
            IMapper mapper)
        {
            _warehouseRepository = warehouseRepository;
            _mapper = mapper;
        }

        public async Task<WarehouseDto> CreateAsync(WarehouseDto warehouseDto)
        {
            if (warehouseDto == null)
            {
                throw new ArgumentNullException(nameof(warehouseDto));
            }

            var existingWarehouse = await _warehouseRepository.FilterAsync(x =>
                x.Name == warehouseDto.Name && x.Company.Id == warehouseDto.Company.Id);
            if (existingWarehouse.Count != 0)
            {
                throw new Exception("Warehouse with same name already exists.");
            }

            var warehouse = _mapper.Map<WarehouseDto, Warehouse>(warehouseDto);

            _warehouseRepository.Add(warehouse);

            await _warehouseRepository.SaveChangesAsync();

            return _mapper.Map<Warehouse, WarehouseDto>(warehouse);
        }

        public async Task<List<WarehouseDto>> GetAllAsync()
        {
            var warehouses = await _warehouseRepository.GetAllAsync();

            return _mapper.Map<List<Warehouse>, List<WarehouseDto>>(warehouses);
        }

        public async Task<WarehouseDto> GetAsync(Guid warehouseId)
        {
            var warehouse = await FindWarehouse(warehouseId);

            return _mapper.Map<Warehouse, WarehouseDto>(warehouse);
        }

        public async Task RemoveAsync(Guid warehouseId)
        {
            var warehouse = await FindWarehouse(warehouseId);

            _warehouseRepository.Remove(warehouse);

            await _warehouseRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(WarehouseDto warehouseDto)
        {
            if (warehouseDto == null)
            {
                throw new ArgumentNullException(nameof(warehouseDto));
            }

            var warehouse = await FindWarehouse(warehouseDto.Id);

            _mapper.Map(warehouseDto, warehouse);

            await _warehouseRepository.SaveChangesAsync();
        }
        
        private async Task<Warehouse> FindWarehouse(Guid warehouseId)
        {
            if (warehouseId == default)
            {
                throw new ArgumentException("Warehouse id is invalid.", nameof(warehouseId));
            }

            var warehouse = await _warehouseRepository.GetAsync(warehouseId);
            if (warehouse == null)
            {
                throw new Exception($"Could not find warehouse with id = '{warehouseId}'.");
            }

            return warehouse;
        }
    }
}