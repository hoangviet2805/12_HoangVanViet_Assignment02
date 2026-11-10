using Microsoft.EntityFrameworkCore;

namespace _12_HoangVanViet_A02_BE.Models
{
    public class FUNewsManagementContext : DbContext
    {
        public FUNewsManagementContext()
        {
        }

        public FUNewsManagementContext(DbContextOptions<FUNewsManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<NewsArticle> NewsArticles { get; set; } = null!;
        public virtual DbSet<NewsTag> NewsTags { get; set; } = null!;
        public virtual DbSet<SystemAccount> SystemAccounts { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NewsTag>(entity =>
            {
                entity.HasKey(e => new { e.NewsArticleID, e.TagID });
            });
        }
    }
}
