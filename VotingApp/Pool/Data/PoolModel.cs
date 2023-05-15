using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VotingApp.Pool.Domain;
namespace VotingApp.Pool.Data;



public class PoolModel : IEntityTypeConfiguration<PoolEntity>
{
    public void Configure(EntityTypeBuilder<PoolEntity> builder)
    {
        builder.ToTable("Pool");
        builder.HasKey(p => p.ID);
        builder.Property(p => p.Question).IsRequired().HasMaxLength(60);
        builder.HasOne(p => p.User).WithMany(u => u.Pools).HasForeignKey(p => p.UserID);
        builder.HasMany(p => p.Options).WithOne(o => o.Pool).HasForeignKey(o => o.PoolID);

    }
}

