using Hackathon.Application.Companies;
using Hackathon.Application.DaliveryPoints;
using Hackathon.Application.Deliveries;
using Hackathon.Application.Warehouses;
using Hackathon.Domain.DeliveryAggregate;
using Hackathon.Domain.UserAggregate;
using Hackathon.MVC.ViewModels;
using Hackathon.MVC.ViewModels.Delivery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackathon.MVC.Controllers
{
    [Authorize]
    public class DeliveriesController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWarehouseService _warehouseService;
        private readonly IDeliveryService _deliveryService;
        private readonly IDeliveryPointService _deliveryPointService;
        private readonly ICompanyService _companyService;

        public DeliveriesController(
            UserManager<User> userManager,
            IWarehouseService warehouseService,
            IDeliveryService deliveryService,
            IDeliveryPointService deliveryPointService,
            ICompanyService companyService)
        {
            _userManager = userManager;
            _warehouseService = warehouseService;
            _deliveryService = deliveryService;
            _deliveryPointService = deliveryPointService;
            _companyService = companyService;
        }

        // GET: Deliveries
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var company = await _companyService.GetAsync((Guid) user.CompanyId);

            var vm = new DeliveryViewModel
            {
                DeliveriesInProgress = await _deliveryService.GetAllAsync(),
                DeliveriesRequests = await _deliveryService.GetAllAsync(),
                DestinationCompanies = company
            };

            return View(vm);
        }

        // GET: Deliveries/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _deliveryService.GetAsync((Guid) id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // GET: Deliveries/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Companies"] = new SelectList(await _companyService.GetAllAsync(), "Id", "Name");
            return View();
        }

        // POST: Deliveries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeliveryDto delivery)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(HttpContext.User);

                    var warehouse = (await _warehouseService.GetAllAsync())
                        .Any(x =>
                            x.CompanyId == delivery?.SourceCompanyId && x.MaxCode >= delivery?.Code &&
                            x.MinCode <= delivery.Code);

                    if (warehouse)
                    {
                        var newDelivery = new DeliveryDto()
                        {
                            Id = Guid.NewGuid(),
                            CreationTime = DateTime.Now,
                            DestinationCompanyId = user.CompanyId,
                            SourceCompanyId = delivery.SourceCompanyId,
                            Code = delivery.Code,
                            Status = DeliveryStatus.None
                        };

                        await _deliveryService.CreateAsync(newDelivery);
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewData["Companies"] = new SelectList(await _companyService.GetAllAsync(), "Id", "Name");
                        ModelState.AddModelError(string.Empty,
                            $"Nobeno skladišče, tega podjetja, ne sprejema distavne šifre {delivery.Code}");
                        return View(delivery);
                    }
                }
                catch (Exception e)
                {
                    ViewData["Companies"] = new SelectList(await _companyService.GetAllAsync(), "Id", "Name");
                    ModelState.AddModelError(string.Empty, "Podjetje s takšnim imenom ne obstaja. Navedite točno ime.");
                    return View(delivery);
                }
            }

            ViewData["Companies"] = new SelectList(await _companyService.GetAllAsync(), "Id", "Name");
            return View(delivery);
        }

        // GET: Deliveries/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _deliveryService.GetAsync((Guid) id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // POST: Deliveries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DeliveryDto delivery)
        {
            if (id != delivery.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var updateDelivery = await _deliveryService.GetAsync(id);

                    updateDelivery.DeliveryTime = delivery.DeliveryTime;
                    updateDelivery.DispatchTime = delivery.DispatchTime;
                    updateDelivery.DeliveryPointId = delivery.DeliveryPointId;

                    await _deliveryService.UpdateAsync(updateDelivery);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DeliveryExists(delivery.Id))
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

            return View(delivery);
        }


        // GET: Deliveries/Delete/5
        public async Task<IActionResult> Dispatch(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _deliveryService.GetAsync((Guid) id);
            var company = await _companyService.GetAsync((Guid) delivery?.DestinationCompanyId);

            try
            {
                var (deliveryPoints, deliveryEvents) = await PrepareDeliveryData(delivery, company);

                return View(PrepareDispatchResponse(delivery, company, deliveryPoints, deliveryEvents));
            }
            catch (Exception e)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Prosimo vnesite skladišča (tudi min in max vrednost posebej) in dostavne točke, ter nato poskusite ponovno."
                );

                return View(
                    PrepareDispatchResponse(delivery, company, new List<DeliveryPointDto>(), new List<Event>())
                );
            }
        }

        private DeliveryViewModel PrepareDispatchResponse(
            DeliveryDto delivery,
            CompanyDto company,
            List<DeliveryPointDto> deliveryPoints,
            List<Event> deliveryEvents)
        {
            return new DeliveryViewModel()
            {
                Id = delivery.Id,
                Code = delivery.Code,
                DestinationCompanies = company,
                DeliveryPoints = deliveryPoints,
                DeliveryEvents = deliveryEvents
            };
        }

        private async Task<Tuple<List<DeliveryPointDto>, List<Event>>> PrepareDeliveryData(DeliveryDto delivery,
            CompanyDto company)
        {
            var warehouse = (await _warehouseService.GetAllAsync())
                .FirstOrDefault(x =>
                    x.CompanyId == company.Id && x.MaxCode >= delivery.Code && x.MinCode <= delivery.Code);

            var deliveryPoints = (await _deliveryPointService.GetAllAsync())
                .Where(x => x.WarehouseId == warehouse?.Id)
                .ToList();

            var deliveryEvents = PrepareDeliveryEvents(deliveryPoints);

            return new Tuple<List<DeliveryPointDto>, List<Event>>(deliveryPoints, deliveryEvents);
        }

        private List<Event> PrepareDeliveryEvents(List<DeliveryPointDto> deliveryPoints)
        {
            var deliveryEvents = new List<Event>();

            deliveryPoints.ForEach((x) =>
            {
                var deliveries = _deliveryService.GetAllAsync().GetAwaiter().GetResult()
                    .Where(o => o.DeliveryPointId == x.Id).ToList();

                deliveries.ForEach(d =>
                {
                    if (d.DispatchTime != null && d.DeliveryTime != null)
                    {
                        deliveryEvents.Add(new Event()
                        {
                            title = "Dostava",
                            start = d.DispatchTime,
                            end = d.DeliveryTime
                        });
                    }
                });
            });

            return deliveryEvents;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dispatch(Guid? id, DeliveryDto delivery)
        {
            if (id != delivery.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var newDelivery = await _deliveryService.GetAsync((Guid) id);

                    newDelivery.DeliveryTime = delivery.DeliveryTime;
                    newDelivery.DispatchTime = delivery.DispatchTime;
                    newDelivery.DeliveryPointId = delivery.DeliveryPointId;
                    newDelivery.Status = DeliveryStatus.InProgress;

                    await _deliveryService.UpdateAsync(newDelivery);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DeliveryExists(delivery.Id))
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

            return View(delivery);
        }


        public async Task<IActionResult> Accept(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _deliveryService.GetAsync((Guid) id);
            if (delivery == null)
            {
                return NotFound();
            }

            try
            {
                delivery.Status = DeliveryStatus.Received;
                await _deliveryService.UpdateAsync(delivery);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await DeliveryExists(delivery.Id))
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

        // GET: Deliveries/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delivery = await _deliveryService.GetAsync((Guid) id);

            await _deliveryService.RemoveAsync(delivery.Id);

            return RedirectToAction(nameof(Index));
        }

        // POST: Deliveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _deliveryService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> DeliveryExists(Guid id)
        {
            return (await _deliveryService.GetAllAsync()).Any(e => e.Id == id);
        }
    }
}