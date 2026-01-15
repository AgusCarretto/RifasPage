using LogicaAccesoDatos.EF;
using LogicaNegocio;
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
            return this.Context.Rifas.ToList();

            }catch (Exception ex)
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

    }
}
