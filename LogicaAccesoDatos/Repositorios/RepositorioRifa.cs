using LogicaAccesoDatos.EF;
using LogicaNegocio;
using LogicaNegocio.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

               
            }

        }

        public void ReservarVariasRifas(string idsRifas, int idComprador)
        {

            List<int> idsRifasSeleccionadas = idsRifas.Split(',')
                                        .Select(int.Parse)
                                        .ToList();
            foreach (int id in idsRifasSeleccionadas)
            {
                ReservarRifa(id, idComprador);
            }


            this.Context.SaveChanges();


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

        public decimal calcularMonto(string idsRifas)
        {
            decimal total = 0;
            List<int> idsRifasSeleccionadas = idsRifas.Split(',')
                                        .Select(int.Parse)
                                        .ToList();
            foreach (int id in idsRifasSeleccionadas)
            {
                total += precioDeUnaRifa(id);
            }
            
            return total;
        
        }


        public decimal precioDeUnaRifa(int id)
        {
            Rifa rifa = this.Context.Rifas.Find(id);

            return rifa.prize;

        }


    }
}
