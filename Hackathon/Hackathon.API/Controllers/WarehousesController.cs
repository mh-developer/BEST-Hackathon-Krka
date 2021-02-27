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

    }
}