using Hackathon.Application.DaliveryPoints;
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
    public class DeliveryPointsController : ControllerBase
    {
        private readonly IDeliveryPointService _deliveryPointService;
        private readonly ILogger<DeliveryPointsController> _logger;

        public DeliveryPointsController(
            IDeliveryPointService deliveryPointService,
            ILogger<DeliveryPointsController> logger)
        {
            _deliveryPointService = deliveryPointService;
            _logger = logger;
        }

        /// <summary>
        ///     List of delivery points
        /// </summary>
        /// <response code="200">Returns list of delivery points</response>
        /// <response code="400">Bad request error massage</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDeliveryPoints()
        {
            try
            {
                _logger.LogInformation("Start getting delivery points.");
                var result = await _deliveryPointService.GetAllAsync();
                if (result == null)
                {
                    _logger.LogError("Error getting delivery points. DeliveryPoints not found.");
                    return NotFound("Error getting delivery points. DeliveryPoints not found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting delivery points.", ex);
                return BadRequest("Error getting delivery points.");
            }
        }

        /// <summary>
        ///     DeliveryPoint object
        /// </summary>
        /// <response code="200">Returns delivery point object</response>
        /// <response code="400">Bad request error massage</response>
        /// <response code="404">Not found delivery point</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDeliveryPoint([FromRoute] Guid id)
        {
            try
            {
                _logger.LogInformation($"Start getting delivery point with ID {id}.");
                var result = await _deliveryPointService.GetAsync(id);
                if (result == null)
                {
                    _logger.LogError("Error getting delivery point. DeliveryPoint not found.");
                    return NotFound("Error getting delivery point. DeliveryPoint not found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting delivery point.", ex);
                return BadRequest("Error getting delivery point.");
            }
        }

        /// <summary>
        ///     Create delivery point
        /// </summary>
        /// <response code="201">Returns the newly created delivery point</response>
        /// <response code="400">If delivery point is null</response>  
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> CreateDeliveryPoint(DeliveryPointDto deliveryPoint)
        {
            try
            {
                _logger.LogInformation("Start creating delivery point.");
                var result = await _deliveryPointService.CreateAsync(deliveryPoint);
                if (result == null)
                {
                    _logger.LogError("Error creating delivery point.");
                    return BadRequest("Error creating delivery point.");
                }

                return CreatedAtAction(nameof(GetDeliveryPoint), new {id = result.Id}, result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating delivery point.", ex);
                return BadRequest("Error creating delivery point.");
            }
        }

        /// <summary>
        ///     Update delivery point
        /// </summary>
        /// <response code="204">No content</response>
        /// <response code="404">If the delivery point is null</response>
        /// <response code="400">Bad request arguments</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> UpdateDeliveryPoint([FromRoute] Guid id,
            [FromBody] DeliveryPointDto deliveryPoint)
        {
            try
            {
                _logger.LogInformation($"Start updating delivery point with ID {id}.");
                if (deliveryPoint.Id != id)
                {
                    _logger.LogError(
                        $"Error updating delivery point. Argument ID exception. {id} != {deliveryPoint.Id}");
                    return BadRequest(
                        $"Error updating delivery point. Argument ID exception. {id} != {deliveryPoint.Id}");
                }

                var result = await _deliveryPointService.GetAsync(id);
                if (result == null)
                {
                    _logger.LogError("Error updating delivery point. DeliveryPoint not found.");
                    return NotFound("Error updating delivery point. DeliveryPoint not found.");
                }

                await _deliveryPointService.UpdateAsync(deliveryPoint);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating delivery point.", ex);
                return BadRequest("Error updating delivery point.");
            }
        }

        /// <summary>
        ///     Delete delivery point
        /// </summary>
        /// <response code="204">No content</response>
        /// <response code="404">If the delivery point is null</response>
        /// <response code="400">Bad request arguments</response> 
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> DeleteDeliveryPoint([FromRoute] Guid id)
        {
            try
            {
                _logger.LogInformation($"Start deleting delivery point with ID {id}.");
                var deliveryPoint = await _deliveryPointService.GetAsync(id);

                if (deliveryPoint == null)
                {
                    _logger.LogError("Error deleting delivery point. DeliveryPoint not found.");
                    return NotFound("Error deleting delivery point. DeliveryPoint not found.");
                }

                await _deliveryPointService.RemoveAsync(deliveryPoint.Id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting delivery point.", ex);
                return BadRequest("Error deleting delivery point.");
            }
        }
    }
}