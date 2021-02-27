using Hackathon.Application.Companies;
using Hackathon.Application.DaliveryPoints;
using Hackathon.Application.Deliveries;
using Hackathon.Application.Warehouses;
using System.Collections.Generic;

namespace Hackathon.MVC.ViewModels.Delivery
{
    public class DeliveryViewModel : DeliveryInputModel
    {
        public IList<DeliveryDto> DeliveriesInProgress { get; set; }

        public IList<DeliveryDto> DeliveriesRequests { get; set; }

        public CompanyDto DestinationCompanies { get; set; }

        public IList<WarehouseDto> Warehouses { get; set; }

        public IList<DeliveryPointDto> DeliveryPoints { get; set; }

        public IList<Event> DeliveryEvents { get; set; }
    }
}