using Microsoft.EntityFrameworkCore;
using P03_FootballBetting.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P03_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        {

        }

        public FootballBettingContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.connectionString);

            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.TeamId);

                entity.Property(t => t.Name)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(50);

                entity.Property(t => t.LogoUrl)
                .IsRequired()
                .IsUnicode(false);

                entity.Property(t => t.Initials)
                .HasMaxLength(3)
                .IsUnicode()
                .IsRequired();

                entity.Property(t => t.Budget)
                .IsRequired();

                entity.HasOne(t => t.PrimaryKitColor)
                .WithMany(c => c.PrimaryKitTeams)
                .HasForeignKey(t => t.PrimaryKitColorId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.SecondaryKitColor)
                .WithMany(c => c.SecondaryKitTeams)
                .HasForeignKey(t => t.SecondaryKitColorId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Color>(entity => {
                entity.HasKey(c => c.ColorId);

                entity.Property(c => c.Name)
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode(false);

            });

            modelBuilder.Entity<Town>(entity =>
            {
                entity.HasKey(t => t.TownId);

                entity.Property(t => t.Name)
                .HasMaxLength(50)
                .IsUnicode()
                .IsRequired();

                entity.HasOne(t => t.Country)
                .WithMany(c => c.Towns)
                .HasForeignKey(t => t.CountryId);

            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(c => c.CountryId);

                entity.Property(c => c.Name)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode(false);
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.PlayerId);

                entity.Property(p => p.Name)
                .HasMaxLength(80)
                .IsRequired()
                .IsUnicode();

                entity.Property(p => p.SquadNumber)
                .IsRequired();

                entity.HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId);

                entity.HasOne(p => p.Position)
                .WithMany(ps => ps.Players)
                .HasForeignKey(p => p.PositionId);

            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasKey(p => p.PositionId);

                entity.Property(p => p.Name)
                .HasMaxLength(20)
                .IsUnicode()
                .IsRequired();
            });

            modelBuilder.Entity<PlayerStatistic>(entity =>
            {
                entity.HasKey(ps => new { ps.PlayerId, ps.GameId });

                entity.HasOne(ps => ps.Player)
                .WithMany(p => p.PlayerStatistics)
                .HasForeignKey(ps => ps.PlayerId);

                entity.HasOne(ps => ps.Game)
                .WithMany(g => g.PlayerStatistics)
                .HasForeignKey(ps => ps.GameId);
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.GameId);

                entity.Property(g => g.Result)
                .IsRequired(false)
                .IsUnicode(false)
                .HasMaxLength(7);

                entity.HasOne(g => g.HomeTeam)
                .WithMany(t => t.HomeGames)
                .HasForeignKey(g => g.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.AwayTeam)
                .WithMany(t => t.AwayGames)
                .HasForeignKey(g => g.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Bet>(entity =>
            {
                entity.HasKey(b => b.BetId);

                entity.HasOne(b => b.User)
                .WithMany(u => u.Bets)
                .HasForeignKey(b => b.BetId);

                entity.HasOne(b => b.Game)
                .WithMany(g => g.Bets)
                .HasForeignKey(b => b.GameId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.Property(u => u.Username)
                .HasMaxLength(30)
                .IsRequired()
                .IsUnicode(false);

                entity.Property(u => u.Password)
                .HasMaxLength(256)
                .IsRequired()
                .IsUnicode(false);

                entity.Property(u => u.Email)
                .HasMaxLength(30)
                .IsUnicode(false)
                .IsRequired();

                entity.Property(u => u.Name)
                .HasMaxLength(30)
                .IsRequired(false)
                .IsUnicode();
            });
        }
    }
}

