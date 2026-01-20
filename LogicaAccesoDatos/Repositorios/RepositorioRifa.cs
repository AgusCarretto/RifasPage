using LogicaAccesoDatos.EF;
using LogicaNegocio;
using LogicaNegocio.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaAccesoDatos.Repositorios
{
    public class RepositorioRifa
    {

        public ProyectoContext Context { get; set; }
        public RepositorioRifa(ProyectoContext context)
        {
            this.Context = context;
        }

        public void AddRifa(Rifa rifa)
        {

        }
        public List<Rifa> ObtenerTodas()
        {
            try
            {
                return this.Context.Rifas
            .Include(r => r.comprador)
            .ToList();

            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las rifas", ex);
            }
        }

        public void ActualizarEstado(int idRifa, Rifa.EstadoRifa nuevoEstado)
        {
            var rifa = Context.Rifas.Find(idRifa);
            if (rifa != null)
            {
                rifa.state = nuevoEstado;
                Context.SaveChanges();
            }
        }


        public void ReservarRifa(int idRifa, int idComprador)
        {

            var rifa = this.Context.Rifas.Find(idRifa);
            if (rifa != null)
            {
                rifa.state = Rifa.EstadoRifa.Reservado;
                rifa.CompradorId = idComprador;

                this.Context.SaveChanges();
            }

        }

        public MejorCompradorDTO ObtenerMejorComprador()
        {
            return this.Context.Rifas
        .Include(r => r.comprador)
        .Where(r => r.comprador != null && r.state != Rifa.EstadoRifa.Disponible)
        .GroupBy(r => r.comprador.name) 
        .Select(g => new MejorCompradorDTO
        {
            Nombre = g.Key,
            Cantidad = g.Count()
        })
        .OrderByDescending(x => x.Cantidad)
        .FirstOrDefault();
        }


    }
}
