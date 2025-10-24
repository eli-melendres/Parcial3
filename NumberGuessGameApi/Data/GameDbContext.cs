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

            //Config entidad Player

            //Config entidad Game

            //Config entidad Attempt
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
