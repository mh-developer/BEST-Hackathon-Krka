using Hackathon.Application.DaliveryPoints;
using Hackathon.Application.Warehouses;
using System.Collections.Generic;

namespace Hackathon.MVC.ViewModels.Warehouses
{
    public class WarehouseViewModel : WarehouseDto
    {
        public IList<WarehouseDto> Warehouses { get; set; }

        public IList<DeliveryPointDto> DeliveryPoints { get; set; }
    }
}