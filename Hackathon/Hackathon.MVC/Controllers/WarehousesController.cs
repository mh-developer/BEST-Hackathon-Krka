using Hackathon.Application.Companies;
using Hackathon.Application.DaliveryPoints;
using Hackathon.Application.Users;
using Hackathon.Application.Warehouses;
using Hackathon.Domain.UserAggregate;
using Hackathon.MVC.ViewModels.Warehouses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hackathon.MVC.Controllers
{
    [Authorize]
    public class WarehousesController : Controller
    {
        private readonly ILogger<WarehousesController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;
        private readonly IWarehouseService _warehouseService;
        private readonly IDeliveryPointService _deliveryPointService;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly ICompanyService _companyService;

        public WarehousesController(
            ILogger<WarehousesController> logger,
            IAuthenticationSchemeProvider schemeProvider,
            UserManager<User> userManager,
            IUserService userService,
            IDeliveryPointService deliveryPointService,
            IWarehouseService warehouseService,
            ICompanyService companyService)
        {
            _logger = logger;
            _userManager = userManager;
            _userService = userService;
            _warehouseService = warehouseService;
            _deliveryPointService = deliveryPointService;
            _schemeProvider = schemeProvider;
            _companyService = companyService;
        }

        // GET: Warehouses
        public async Task<IActionResult> Index()
        {
            return View(await _warehouseService.GetAllAsync());
        }

        // GET: Warehouses/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _warehouseService.GetAsync((Guid) id);
            var deliveryPoints = (await _deliveryPointService.GetAllAsync()).Where(x => x.WarehouseId == warehouse.Id)
                .ToList();

            var vm = new WarehouseViewModel()
            {
                DeliveryPoints = deliveryPoints
            };

            if (warehouse == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        // GET: Warehouses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Warehouses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseDto warehouse)
        {
            if (ModelState.IsValid)
            {
                warehouse.Id = Guid.NewGuid();

                var user = await _userManager.GetUserAsync(HttpContext.User);
                var company = await _companyService.GetAsync((Guid) user.CompanyId);
                warehouse.CompanyId = company.Id;


                await _warehouseService.CreateAsync(warehouse);
                return RedirectToAction(nameof(Index));
            }

            return View(warehouse);
        }

        // GET: Warehouses/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _warehouseService.GetAsync((Guid) id);
            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse);
        }

        // POST: Warehouses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("MinCode,MaxCode,Name,Address,PhoneNumber,Id")]
            WarehouseDto warehouse)
        {
            if (id != warehouse.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(HttpContext.User);
                    var company = await _companyService.GetAsync((Guid) user.CompanyId);
                    warehouse.CompanyId = company.Id;

                    await _warehouseService.UpdateAsync(warehouse);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await WarehouseExists(warehouse.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(warehouse);
        }

        // GET: Warehouses/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _warehouseService.GetAsync((Guid) id);
            await _warehouseService.RemoveAsync(warehouse.Id);

            return RedirectToAction(nameof(Index));
        }

        // POST: Warehouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _warehouseService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> WarehouseExists(Guid id)
        {
            var warehouse = await _warehouseService.GetAllAsync();
            return warehouse.Any(e => e.Id == id);
        }
    }
}