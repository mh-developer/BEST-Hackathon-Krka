using Hackathon.Domain.DeliveryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackathon.Infrastructure.EntityTypes
{
    internal class DeliveryEntityTypeConfiguration : BaseEntityTypeConfiguration<Delivery>
    {
        public override void Configure(EntityTypeBuilder<Delivery> builder)
        {
            builder.ToTable("Delivery");

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasColumnName("DeliveryID");

            builder.HasOne(o => o.DeliveryPoint);
            builder.HasOne(o => o.SourceCompany);
            builder.HasOne(o => o.DestinationCompany);

            builder.Property(o => o.Code).IsRequired();
            builder.Property(o => o.Status).HasDefaultValue(DeliveryStatus.None).HasConversion<byte>();
            builder.Property(o => o.DispatchTime).IsRequired(false);
            builder.Property(o => o.DeliveryTime).IsRequired(false);
            builder.Property(o => o.CreationTime).IsRequired();

            builder.HasIndex(i => i.Id).IsUnique().HasFilter("[IsDeleted] = 0");

            base.Configure(builder);
        }
    }
}