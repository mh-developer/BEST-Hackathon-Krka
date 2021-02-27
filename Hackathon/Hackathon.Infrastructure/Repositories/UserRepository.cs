using Hackathon.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackathon.Infrastructure.Repositories
{
    public class UserRepository : Repository<User, Guid>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<List<User>> GetAllAsync()
        {
            return await Context.Users.Include(x => x.Company).Include(x => x.Warehouse).ToListAsync();
        }

        public override async Task<List<User>> FilterAsync(Func<User, bool> predicate)
        {
            return await Context.Users.Include(x => x.Company).Include(x => x.Warehouse).ToListAsync()
                .ContinueWith(x => x.Result.Where(predicate).ToList());
        }

        public override User Get(Guid id)
        {
            return Context.Users.Include(x => x.Company).Include(x => x.Warehouse).FirstOrDefault(x => x.Id == id);
        }

        public override async Task<User> GetAsync(Guid id)
        {
            return await Context.Users.Include(x => x.Company).Include(x => x.Warehouse)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}