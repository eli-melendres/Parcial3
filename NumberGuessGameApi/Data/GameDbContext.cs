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
        }

    }
}
