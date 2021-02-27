using Hackathon.Application.Deliveries;
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
    public class DeliveriesController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;
        private readonly ILogger<DeliveriesController> _logger;

        public DeliveriesController(IDeliveryService deliveryService, ILogger<DeliveriesController> logger)
        {
            _deliveryService = deliveryService;
            _logger = logger;
        }

        /// <summary>
        ///     List of deliveries
        /// </summary>
        /// <response code="200">Returns list of deliveries</response>
        /// <response code="400">Bad request error massage</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDeliveries()
        {
            try
            {
                _logger.LogInformation("Start getting deliveries.");
                var result = await _deliveryService.GetAllAsync();
                if (result == null)
                {
                    _logger.LogError("Error getting deliveries. Deliveries not found.");
                    return NotFound("Error getting deliveries. Deliveries not found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting deliveries.", ex);
                return BadRequest("Error getting deliveries.");
            }
        }

        /// <summary>
        ///     Delivery object
        /// </summary>
        /// <response code="200">Returns delivery object</response>
        /// <response code="400">Bad request error massage</response>
        /// <response code="404">Not found delivery</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDelivery([FromRoute] Guid id)
        {
            try
            {
                _logger.LogInformation($"Start getting delivery with ID {id}.");
                var result = await _deliveryService.GetAsync(id);
                if (result == null)
                {
                    _logger.LogError("Error getting delivery. Delivery not found.");
                    return NotFound("Error getting delivery. Delivery not found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting delivery.", ex);
                return BadRequest("Error getting delivery.");
            }
        }

        /// <summary>
        ///     Create delivery
        /// </summary>
        /// <response code="201">Returns the newly created delivery</response>
        /// <response code="400">If delivery is null</response>  
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> CreateDelivery(DeliveryDto delivery)
        {
            try
            {
                _logger.LogInformation("Start creating delivery.");
                var result = await _deliveryService.CreateAsync(delivery);
                if (result == null)
                {
                    _logger.LogError("Error creating delivery.");
                    return BadRequest("Error creating delivery.");
                }

                return CreatedAtAction(nameof(GetDelivery), new {id = result.Id}, result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating delivery.", ex);
                return BadRequest("Error creating delivery.");
            }
        }

        /// <summary>
        ///     Update delivery
        /// </summary>
        /// <response code="204">No content</response>
        /// <response code="404">If the delivery is null</response>
        /// <response code="400">Bad request arguments</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> UpdateDelivery([FromRoute] Guid id, [FromBody] DeliveryDto deliveryIn)
        {
            try
            {
                _logger.LogInformation($"Start updating delivery with ID {id}.");
                if (deliveryIn.Id != id)
                {
                    _logger.LogError($"Error updating delivery. Argument ID exception. {id} != {deliveryIn.Id}");
                    return BadRequest($"Error updating delivery. Argument ID exception. {id} != {deliveryIn.Id}");
                }

                var result = await _deliveryService.GetAsync(id);
                if (result == null)
                {
                    _logger.LogError("Error updating delivery. Delivery not found.");
                    return NotFound("Error updating delivery. Delivery not found.");
                }

                await _deliveryService.UpdateAsync(deliveryIn);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating delivery.", ex);
                return BadRequest("Error updating delivery.");
            }
        }

        /// <summary>
        ///     Delete delivery
        /// </summary>
        /// <response code="204">No content</response>
        /// <response code="404">If the delivery is null</response>
        /// <response code="400">Bad request arguments</response> 
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> DeleteDelivery([FromRoute] Guid id)
        {
            try
            {
                _logger.LogInformation($"Start deleting delivery with ID {id}.");
                var delivery = await _deliveryService.GetAsync(id);

                if (delivery == null)
                {
                    _logger.LogError("Error deleting delivery. Delivery not found.");
                    return NotFound("Error deleting delivery. Delivery not found.");
                }

                await _deliveryService.RemoveAsync(delivery.Id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting delivery.", ex);
                return BadRequest("Error deleting delivery.");
            }
        }
    }
}