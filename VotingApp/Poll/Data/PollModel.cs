using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VotingApp.Poll.Domain;
namespace VotingApp.Poll.Data;



public class PollModel : IEntityTypeConfiguration<PollEntity>
{
    public void Configure(EntityTypeBuilder<PollEntity> builder)
    {
        builder.ToTable("Poll");
        builder.HasKey(p => p.ID);
        builder.Property(p => p.Question).IsRequired().HasMaxLength(60);
        builder.HasOne(p => p.User).WithMany(u => u.Polls).HasForeignKey(p => p.UserID);
        builder.HasMany(p => p.Options).WithOne(o => o.Poll).HasForeignKey(o => o.PollID);

    }
}

