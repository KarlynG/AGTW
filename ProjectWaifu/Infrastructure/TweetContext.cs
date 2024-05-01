using Microsoft.EntityFrameworkCore;

namespace ProjectWaifu.Infrastructure;

public class TweetContext : DbContext
{
    public DbSet<WaifuTweet> Tweets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=tweets.db");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WaifuTweet>()
            .HasKey(p => p.Id);
    }
}
