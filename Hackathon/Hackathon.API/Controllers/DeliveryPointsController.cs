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

    }
}