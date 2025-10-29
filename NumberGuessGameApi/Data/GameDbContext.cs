using Microsoft.EntityFrameworkCore;
using NumberGuessGameApi.Models;

namespace NumberGuessGameApi.Data 
{
    //Constructor que recibe las opciones de configuración
    public class GameDbContext : DbContext
    {
        //Cada DbSet<Entity> se convierte en una tabla en SQL Server
        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Attempt> Attempts { get; set; }


        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Config entidad Player
            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("Players");
                entity.HasKey(p => p.PlayerId);
                entity.Property(p => p.PlayerId).ValueGeneratedOnAdd();
                entity.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(p => p.LastName).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Age).IsRequired();
                entity.Property(p => p.RegisteredAt).IsRequired().HasDefaultValueSql("GETDATE()");

                entity.HasMany(p => p.Games)
                    .WithOne(g => g.Player)
                    .HasForeignKey(g => g.PlayerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Config entidad Game
            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable("Games");
                entity.HasKey(g => g.GameId);
                entity.Property(g => g.GameId).ValueGeneratedOnAdd();
                entity.Property(g => g.PlayerId).IsRequired();
                entity.Property(g => g.SecretNumber).IsRequired().HasMaxLength(4);
                entity.Property(g => g.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
                entity.Property(g => g.IsFinished).IsRequired().HasDefaultValue(false);

                entity.HasMany(g => g.Attempts)
                    .WithOne(a => a.Game)
                    .HasForeignKey(a => a.GameId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<Attempt>(entity =>
            {
                entity.ToTable("Attempts");

                entity.HasKey(a => a.AttemptId);

                entity.Property(a => a.AttemptId)
                    .HasColumnName("AttemptId")
                    .ValueGeneratedOnAdd();

                entity.Property(a => a.GameId)
                    .IsRequired()
                    .HasColumnName("GameId");

                entity.Property(a => a.AttemptedNumber)
                    .IsRequired()
                    .HasMaxLength(4)
                    .HasColumnName("AttemptedNumber");

                entity.Property(a => a.Famas)
                    .IsRequired()
                    .HasColumnName("Famas");

                entity.Property(a => a.Picas)
                    .IsRequired()
                    .HasColumnName("Picas");

                entity.Property(a => a.AttemptedAt)
                    .IsRequired()
                    .HasColumnName("AttemptedAt")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(a => a.ResultMessage)
                    .HasMaxLength(500)
                    .HasColumnName("ResultMessage"); 
            });
        }

    }
}
