using Microsoft.EntityFrameworkCore;
using appwebProgetto.Models;

namespace appwebProgetto.Data
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options)
        { }

        public DbSet<Utente> Utenti { get; set; }

        public DbSet<Prodotto> Prodotti { get; set; }

        public DbSet<Ordine> Ordini { get; set; }

        public DbSet<OrdineProdotto> OrdiniProdotti { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Utente>().ToTable(nameof(Utente));
            modelBuilder.Entity<Prodotto>().ToTable(nameof(Prodotto));
            modelBuilder.Entity<Ordine>().ToTable(nameof(Ordine));
            modelBuilder.Entity<OrdineProdotto>().ToTable(nameof(OrdineProdotto));
        }
    }
}
