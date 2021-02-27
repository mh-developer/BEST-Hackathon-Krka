using Hackathon.Domain.DeliveryAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackathon.Infrastructure.Repositories
{
    public class DeliveryPointRepository : Repository<DeliveryPoint, Guid>, IDeliveryPointRepository
    {
        public DeliveryPointRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<List<DeliveryPoint>> GetAllAsync()
        {
            return await Context.DeliveryPoints.Include(x => x.Warehouse).ToListAsync();
        }

        public override async Task<List<DeliveryPoint>> FilterAsync(Func<DeliveryPoint, bool> predicate)
        {
            return await Context.DeliveryPoints.Include(x => x.Warehouse).ToListAsync()
                .ContinueWith(x => x.Result.Where(predicate).ToList());
        }

        public override DeliveryPoint Get(Guid id)
        {
            return Context.DeliveryPoints.Include(x => x.Warehouse).FirstOrDefault(x => x.Id == id);
        }

        public override async Task<DeliveryPoint> GetAsync(Guid id)
        {
            return await Context.DeliveryPoints.Include(x => x.Warehouse).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}