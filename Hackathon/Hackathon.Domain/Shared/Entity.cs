using System;

namespace Hackathon.Domain.Shared
{
    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>, IHasDeletionTime
    {
        public TPrimaryKey Id { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletionTime { get; set; }
    }
}