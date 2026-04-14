using Microsoft.EntityFrameworkCore;
using TestTask_ChatApp.Data.Models;

namespace TestTask_ChatApp.Data
{
    public class ChatDbContext(DbContextOptions<ChatDbContext> options) : DbContext(options)
    {
        public DbSet<ChatMessage> Messages { get; set; }
        public DbSet<SentimentResult> SentimentResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatMessage>(e =>
            {
                e.HasKey(m => m.Id);
                e.Property(m => m.Username).HasMaxLength(50).IsRequired();
                e.Property(m => m.Content).HasMaxLength(2000).IsRequired();
                e.Property(m => m.Room).HasMaxLength(50).HasDefaultValue("general");
                e.HasOne(m => m.Sentiment)
                    .WithOne(s => s.Message)
                    .HasForeignKey<SentimentResult>(s => s.MessageId);
            });

            modelBuilder.Entity<SentimentResult>(e =>
            {
                e.HasKey(s => s.Id);
                e.Property(s => s.Label).HasMaxLength(20).IsRequired();
            });
        }
    }
}
