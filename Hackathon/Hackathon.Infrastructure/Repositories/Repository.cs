using Hackathon.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackathon.Infrastructure.Repositories
{
    public class Repository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected readonly ApplicationDbContext Context;

        public virtual DbSet<TEntity> Table => Context.Set<TEntity>();


        public Repository(ApplicationDbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await Context.Set<TEntity>().ToListAsync();
        }

        public virtual async Task<List<TEntity>> FilterAsync(Func<TEntity, bool> predicate)
        {
            return await Context.Set<TEntity>().ToListAsync().ContinueWith(x => x.Result.Where(predicate).ToList());
        }

        public virtual TEntity Get(TPrimaryKey id)
        {
            return Context.Set<TEntity>().Find(id);
        }

        public virtual async Task<TEntity> GetAsync(TPrimaryKey id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }

        public virtual void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }

        public void AddOrUpdate(TEntity entity)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                Add(entity);
            }
            else
            {
                Update(entity);
            }
        }

        public virtual void Update(TEntity entity)
        {
            Context.Set<TEntity>().Update(entity);
        }

        public virtual void Remove(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
        }

        public virtual void Remove(TPrimaryKey id)
        {
            var entity = Context.Set<TEntity>().Find(id);

            Context.Set<TEntity>().Remove(entity);
        }

        public async Task SaveChangesAsync() => await Context.SaveChangesAsync();

        public void SaveChanges() => Context.SaveChanges();
    }
}