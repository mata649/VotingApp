using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VotingApp.Option.Domain;
namespace VotingApp.Option.Data;



public class OptionModel : IEntityTypeConfiguration<OptionEntity>
{
    public void Configure(EntityTypeBuilder<OptionEntity> builder)
    {
        builder.ToTable("Option");
        builder.HasKey(o => o.ID);
        builder.Property(o => o.Text).IsRequired().HasMaxLength(60);
        builder.HasOne(o => o.Poll).WithMany(o => o.Options).HasForeignKey(o => o.PollID);
        builder.HasMany(o => o.Votes).WithOne(o => o.Option).HasForeignKey(o => o.OptionID);
    }
}

