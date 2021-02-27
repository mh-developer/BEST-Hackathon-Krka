using Hackathon.Domain.WarehouseAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackathon.Infrastructure.Repositories
{
    public class WarehouseRepository : Repository<Warehouse, Guid>, IWarehouseRepository
    {
        public WarehouseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<List<Warehouse>> GetAllAsync()
        {
            return await Context.Warehouses.Include(x => x.Company).ToListAsync();
        }

        public override async Task<List<Warehouse>> FilterAsync(Func<Warehouse, bool> predicate)
        {
            return await Context.Warehouses.Include(x => x.Company).ToListAsync()
                .ContinueWith(x => x.Result.Where(predicate).ToList());
        }

        public override Warehouse Get(Guid id)
        {
            return Context.Warehouses.Include(x => x.Company).FirstOrDefault(x => x.Id == id);
        }

        public override async Task<Warehouse> GetAsync(Guid id)
        {
            return await Context.Warehouses.Include(x => x.Company).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}