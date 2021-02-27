using Hackathon.Application.Warehouses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Hackathon.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        private readonly ILogger<WarehousesController> _logger;

        public WarehousesController(IWarehouseService warehouseService, ILogger<WarehousesController> logger)
        {
            _warehouseService = warehouseService;
            _logger = logger;
        }

        /// <summary>
        ///     List of warehouses
        /// </summary>
        /// <response code="200">Returns list of warehouses</response>
        /// <response code="400">Bad request error massage</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWarehouses()
        {
            try
            {
                _logger.LogInformation("Start getting warehouses.");
                var result = await _warehouseService.GetAllAsync();
                if (result == null)
                {
                    _logger.LogError("Error getting warehouses. Warehouses not found.");
                    return NotFound("Error getting warehouses. Warehouses not found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting warehouses.", ex);
                return BadRequest("Error getting warehouses.");
            }
        }

        /// <summary>
        ///     Warehouse object
        /// </summary>
        /// <response code="200">Returns warehouse object</response>
        /// <response code="400">Bad request error massage</response>
        /// <response code="404">Not found warehouse</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWarehouse([FromRoute] Guid id)
        {
            try
            {
                _logger.LogInformation($"Start getting warehouse with ID {id}.");
                var result = await _warehouseService.GetAsync(id);
                if (result == null)
                {
                    _logger.LogError("Error getting warehouse. Warehouse not found.");
                    return NotFound("Error getting warehouse. Warehouse not found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting warehouse.", ex);
                return BadRequest("Error getting warehouse.");
            }
        }

        /// <summary>
        ///     Create warehouse
        /// </summary>
        /// <response code="201">Returns the newly created warehouse</response>
        /// <response code="400">If warehouse is null</response>  
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> CreateWarehouse(WarehouseDto warehouse)
        {
            try
            {
                _logger.LogInformation("Start creating warehouse.");
                var result = await _warehouseService.CreateAsync(warehouse);
                if (result == null)
                {
                    _logger.LogError("Error creating warehouse.");
                    return BadRequest("Error creating warehouse.");
                }

                return CreatedAtAction(nameof(GetWarehouse), new {id = result.Id}, result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating warehouse.", ex);
                return BadRequest("Error creating warehouse.");
            }
        }

        /// <summary>
        ///     Update warehouse
        /// </summary>
        /// <response code="204">No content</response>
        /// <response code="404">If the warehouse is null</response>
        /// <response code="400">Bad request arguments</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> UpdateWarehouse([FromRoute] Guid id, [FromBody] WarehouseDto warehouseIn)
        {
            try
            {
                _logger.LogInformation($"Start updating warehouse with ID {id}.");
                if (warehouseIn.Id != id)
                {
                    _logger.LogError($"Error updating warehouse. Argument ID exception. {id} != {warehouseIn.Id}");
                    return BadRequest($"Error updating warehouse. Argument ID exception. {id} != {warehouseIn.Id}");
                }

                var result = await _warehouseService.GetAsync(id);
                if (result == null)
                {
                    _logger.LogError("Error updating warehouse. Warehouse not found.");
                    return NotFound("Error updating warehouse. Warehouse not found.");
                }

                await _warehouseService.UpdateAsync(warehouseIn);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating warehouse.", ex);
                return BadRequest("Error updating warehouse.");
            }
        }

        /// <summary>
        ///     Delete warehouse
        /// </summary>
        /// <response code="204">No content</response>
        /// <response code="404">If the warehouse is null</response>
        /// <response code="400">Bad request arguments</response> 
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> DeleteWarehouse([FromRoute] Guid id)
        {
            try
            {
                _logger.LogInformation($"Start deleting warehouse with ID {id}.");
                var warehouse = await _warehouseService.GetAsync(id);

                if (warehouse == null)
                {
                    _logger.LogError("Error deleting warehouse. Warehouse not found.");
                    return NotFound("Error deleting warehouse. Warehouse not found.");
                }

                await _warehouseService.RemoveAsync(warehouse.Id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting warehouse.", ex);
                return BadRequest("Error deleting warehouse.");
            }
        }
    }
}