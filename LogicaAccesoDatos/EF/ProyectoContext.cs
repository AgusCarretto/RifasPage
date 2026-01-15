using LogicaNegocio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaAccesoDatos.EF
{
    public class ProyectoContext : DbContext
    {

        public DbSet<Comprador> Compradores { get; set; }
        public DbSet<Rifa> Rifas { get; set; }
        public DbSet<Admin> Admin { get; set; }

        public ProyectoContext(DbContextOptions<ProyectoContext> options) : base(options) { }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            string cadenaConexion =
             @"SERVER=localhost;
            DATABASE=RifasLondres;
            Trusted_Connection=True;";
            
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rifa>()
            .HasOne(o => o.comprador)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);


        }





        }
}
