using Hackathon.Domain.WarehouseAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackathon.Infrastructure.EntityTypes
{
    internal class WarehouseEntityTypeConfiguration : BaseEntityTypeConfiguration<Warehouse>
    {
        public override void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("Warehouse");

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasColumnName("WarehouseID");

            builder.Property(o => o.Name).IsRequired();
            builder.Property(o => o.Address);
            builder.Property(o => o.PhoneNumber);
            builder.Property(o => o.MinCode).IsRequired(false);
            builder.Property(o => o.MaxCode).IsRequired(false);

            builder.HasOne(o => o.Company);

            builder.HasIndex(i => i.Name).IsUnique().HasFilter("[IsDeleted] = 0");
            builder.HasIndex(i => i.Id).IsUnique().HasFilter("[IsDeleted] = 0");

            base.Configure(builder);
        }
    }
}