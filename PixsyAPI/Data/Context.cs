using Microsoft.EntityFrameworkCore;
using PixsyAPI.Models;

namespace PixsyAPI.Data;

public class PixsyDbContext : DbContext
{
    public PixsyDbContext(DbContextOptions<PixsyDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<Picture> Pictures => Set<Picture>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<PictureLike> PictureLikes => Set<PictureLike>();
    public DbSet<PictureComment> PictureComments => Set<PictureComment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasKey(u => u.UserID);

        modelBuilder.Entity<Board>()
            .HasKey(b => b.BoardID);

        modelBuilder.Entity<Picture>()
            .HasKey(p => p.PictureID);

        modelBuilder.Entity<Tag>()
            .HasKey(t => t.TagID);

        modelBuilder.Entity<PictureLike>()
            .HasKey(l => l.PictureLikeID);

        modelBuilder.Entity<PictureComment>()
            .HasKey(c => c.PictureCommentID);

        modelBuilder.Entity<Board>()
            .Property(b => b.BoardVisibility)
            .HasConversion<int>();

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<int>();

        modelBuilder.Entity<PictureLike>()
            .HasIndex(l => new { l.PictureID, l.UserID })
            .IsUnique();

        modelBuilder.Entity<PictureLike>()
            .HasIndex(l => l.PictureID);

        modelBuilder.Entity<PictureComment>()
            .HasIndex(c => c.PictureID);

        modelBuilder.Entity<PictureComment>()
            .Property(c => c.Content)
            .HasMaxLength(1000);
    }
}
