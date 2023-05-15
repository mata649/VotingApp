using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VotingApp.Vote.Domain;
namespace VotingApp.Vote.Data;

public class VoteModel : IEntityTypeConfiguration<VoteEntity>
{
    public void Configure(EntityTypeBuilder<VoteEntity> builder)
    {
        builder.ToTable("Vote");
        builder.HasKey(v => v.ID);
        builder.HasOne(v => v.User).WithMany(u => u.Votes).HasForeignKey(v => v.UserID);
        builder.HasOne(v => v.Option).WithMany(o => o.Votes).HasForeignKey(v => v.OptionID);
    }
}

