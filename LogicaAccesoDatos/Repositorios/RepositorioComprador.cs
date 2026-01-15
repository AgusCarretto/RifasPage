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
    public class RepositorioComprador
    {
        public ProyectoContext Context { get; set; }
        public RepositorioComprador(ProyectoContext context)
        {
            this.Context = context;
        }

        public int Agregar(Comprador nuevo)
        {
            this.Context.Compradores.Add(nuevo);
            this.Context.SaveChanges();
            return nuevo.id;
        }


    }
}
