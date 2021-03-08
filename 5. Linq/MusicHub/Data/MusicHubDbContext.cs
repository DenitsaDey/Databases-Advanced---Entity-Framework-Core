namespace MusicHub.Data
{
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class MusicHubDbContext : DbContext
    {
        public MusicHubDbContext()
        {
        }

        public MusicHubDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Performer> Performers { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<SongPerformer> SongPerformers { get; set; }
        public DbSet<Writer> Writers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Album>(album =>
            {
                album.HasKey(a => a.Id);

                album.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode();

                album.Property(a => a.ReleaseDate)
                .IsRequired();

                album.HasOne(a => a.Producer)
                .WithMany(p => p.Albums)
                .HasForeignKey(a => a.ProducerId);
            });

            builder.Entity<Performer>(performer =>
            {
                performer.HasKey(p => p.Id);

                performer.Property(p => p.FirstName)
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode();

                performer.Property(p => p.LastName)
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode();

                performer.Property(p => p.Age)
                .IsRequired();

                performer.Property(p => p.NetWorth)
                .IsRequired();
            });

            builder.Entity<Producer>(producer =>
            {
                producer.HasKey(p => p.Id);

                producer.Property(p => p.Name)
                .HasMaxLength(30)
                .IsRequired()
                .IsUnicode();
            });

            builder.Entity<Song>(song =>
            {
                song.HasKey(s => s.Id);

                song.Property(s => s.Name)
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode();

                song.Property(s => s.Duration)
                .IsRequired();

                song.Property(s => s.CreatedOn)
                .IsRequired();

                song.Property(s => s.Genre)
                .IsRequired();

                song.Property(s => s.Price)
                .IsRequired();

                song.HasOne(s => s.Album)
                .WithMany(a => a.Songs)
                .HasForeignKey(s => s.AlbumId);

                song.HasOne(s => s.Writer)
                .WithMany(w => w.Songs)
                .HasForeignKey(s => s.WriterId);
            });

            builder.Entity<SongPerformer>(entity =>
            {
                entity.HasKey(sp => new { sp.SongId, sp.PerformerId });

                entity.HasOne(sp => sp.Song)
                .WithMany(s => s.SongPerformers)
                .HasForeignKey(sp => sp.SongId);

                entity.HasOne(sp => sp.Performer)
                .WithMany(p => p.PerformerSongs)
                .HasForeignKey(sp => sp.PerformerId);
            });

            builder.Entity<Writer>(writer =>
            {
                writer.HasKey(w => w.Id);

                writer.Property(w => w.Name)
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode();

            });
        }
    }
}
