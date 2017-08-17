using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using Upo.Model;

namespace Upo.Data
{
    public partial class UpoECommerceContext : DbContext
    {
        //Setup connection string dovrebbe avvenire qua (Startup.ConfigureServices: AddDbContext chiama questo)
        //ma questo costruttore non viene mai chiamato. Connessione risolta in OnConfiguring, piu' in basso
        public UpoECommerceContext(DbContextOptions<UpoECommerceContext> options) : base(options)
        {
        }

        public UpoECommerceContext() { }

        public virtual DbSet<Ordine> Ordine { get; set; }
        public virtual DbSet<OrdineProdotto> OrdineProdotto { get; set; }
        public virtual DbSet<Prodotto> Prodotto { get; set; }
        public virtual DbSet<Utente> Utente { get; set; }

        //Connection string viene settata qua: usa le configurations create in Startup (public e static)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = Startup.Configuration.GetConnectionString("DefaultConnection");
 
            optionsBuilder.UseSqlServer(connectionString);
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ordine>(entity =>
            {
                entity.HasKey(e => e.CdOrdine)
                    .HasName("PK_Ordine");

                entity.Property(e => e.CdOrdine).HasColumnName("CD_ORDINE");

                entity.Property(e => e.CdUtente).HasColumnName("CD_UTENTE");

                entity.Property(e => e.DtInserimento)
                    .HasColumnName("dt_inserimento")
                    .HasColumnType("date");

                entity.Property(e => e.Stato)
                    .HasColumnName("stato")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.Totale).HasColumnName("totale");

                entity.HasOne(d => d.CdUtenteNavigation)
                    .WithMany(p => p.Ordine)
                    .HasForeignKey(d => d.CdUtente)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Ordine_Utente");
            });

            modelBuilder.Entity<OrdineProdotto>(entity =>
            {
                entity.HasKey(e => new { e.CdOrdine, e.CdProdotto })
                    .HasName("PK_Ordine_Prodotto");

                entity.ToTable("Ordine_Prodotto");

                entity.Property(e => e.CdOrdine).HasColumnName("CD_ORDINE");

                entity.Property(e => e.CdProdotto).HasColumnName("CD_PRODOTTO");

                entity.Property(e => e.Quantita).HasColumnName("quantita");

                entity.HasOne(d => d.CdOrdineNavigation)
                    .WithMany(p => p.OrdineProdotto)
                    .HasForeignKey(d => d.CdOrdine)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Ordine_Prodotto_Ordine");

                entity.HasOne(d => d.CdProdottoNavigation)
                    .WithMany(p => p.OrdineProdotto)
                    .HasForeignKey(d => d.CdProdotto)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Ordine_Prodotto_Prodotto");
            });

            modelBuilder.Entity<Prodotto>(entity =>
            {
                entity.HasKey(e => e.CdProdotto)
                    .HasName("PK_Prodotto");

                entity.Property(e => e.CdProdotto).HasColumnName("CD_PRODOTTO");

                entity.Property(e => e.Descrizione)
                    .IsRequired()
                    .HasColumnName("descrizione")
                    .HasColumnType("text");

                entity.Property(e => e.Disponibile)
                    .IsRequired()
                    .HasColumnName("disponibile")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.Immagine).HasColumnName("immagine");

                entity.Property(e => e.Prezzo).HasColumnName("prezzo");

                entity.Property(e => e.Sconto).HasColumnName("sconto");

                entity.Property(e => e.Titolo)
                    .IsRequired()
                    .HasColumnName("titolo")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<Utente>(entity =>
            {
                entity.HasKey(e => e.CdUtente)
                    .HasName("PK_Utente");

                entity.HasIndex(e => e.Username)
                    .HasName("UQ__Utente__F3DBC57291EBE031")
                    .IsUnique();

                entity.Property(e => e.CdUtente).HasColumnName("CD_UTENTE");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Ruolo)
                    .HasColumnName("ruolo")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasColumnType("varchar(50)");
            });
        }
    }
}