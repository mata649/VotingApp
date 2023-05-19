using Microsoft.EntityFrameworkCore;
using VotingApp.User.Domain;
using VotingApp.User.Data;
using VotingApp.Poll.Domain;
using VotingApp.Option.Domain;
using VotingApp.Option.Data;
using VotingApp.Vote.Domain;
using VotingApp.Vote.Data;
using VotingApp.Poll.Data;

namespace VotingApp.Context;

public class VotingAppContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<PollEntity> Polls { get; set; }
    public DbSet<OptionEntity> Options { get; set; }
    public DbSet<VoteEntity> Votes { get; set; }

    public VotingAppContext(DbContextOptions<VotingAppContext> options) : base(options) { }

    public VotingAppContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new UserModel().Configure(modelBuilder.Entity<UserEntity>());
        new PollModel().Configure(modelBuilder.Entity<PollEntity>());
        new OptionModel().Configure(modelBuilder.Entity<OptionEntity>());
        new VoteModel().Equals(modelBuilder.Entity<VoteEntity>());
    }
}

