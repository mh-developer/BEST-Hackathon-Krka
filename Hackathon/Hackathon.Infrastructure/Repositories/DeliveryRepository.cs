using Hackathon.Domain.DeliveryAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackathon.Infrastructure.Repositories
{
    public class DeliveryRepository : Repository<Delivery, Guid>, IDeliveryRepository
    {
        public DeliveryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<List<Delivery>> GetAllAsync()
        {
            return await Context.Deliveries
                .Include(x => x.SourceCompany)
                .Include(x => x.DestinationCompany)
                .Include(x => x.DeliveryPoint)
                .Include(x => x.DeliveryPoint.Warehouse).ToListAsync();
        }

        public override async Task<List<Delivery>> FilterAsync(Func<Delivery, bool> predicate)
        {
            return await Context.Deliveries
                .Include(x => x.SourceCompany)
                .Include(x => x.DestinationCompany)
                .Include(x => x.DeliveryPoint)
                .Include(x => x.DeliveryPoint.Warehouse).ToListAsync()
                .ContinueWith(x => x.Result.Where(predicate).ToList());
        }

        public override Delivery Get(Guid id)
        {
            return Context.Deliveries
                .Include(x => x.SourceCompany)
                .Include(x => x.DestinationCompany)
                .Include(x => x.DeliveryPoint)
                .Include(x => x.DeliveryPoint.Warehouse).FirstOrDefault(x => x.Id == id);
        }

        public override async Task<Delivery> GetAsync(Guid id)
        {
            return await Context.Deliveries
                .Include(x => x.SourceCompany)
                .Include(x => x.DestinationCompany)
                .Include(x => x.DeliveryPoint)
                .Include(x => x.DeliveryPoint.Warehouse).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}