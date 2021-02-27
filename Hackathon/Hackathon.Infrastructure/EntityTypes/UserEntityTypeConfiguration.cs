using Hackathon.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackathon.Infrastructure.EntityTypes
{
    internal class UserEntityTypeConfiguration : BaseEntityTypeConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnName("UserID");

            builder.Property(u => u.FirstName);
            builder.Property(u => u.LastName);

            builder.HasOne(u => u.Company);
            builder.HasOne(u => u.Warehouse);

            builder.HasIndex(i => i.NormalizedUserName).IsUnique().HasFilter("[IsDeleted] = 0");
            builder.HasIndex(i => i.UserName).IsUnique().HasFilter("[IsDeleted] = 0");

            base.Configure(builder);
        }
    }
}