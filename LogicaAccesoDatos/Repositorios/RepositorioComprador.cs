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

            Comprador comprador = findByNumber(nuevo.phoneNumber);

            if (comprador != null)
            {
                return comprador.id;
            }
            else
            {
                nuevo.Validar();
                this.Context.Compradores.Add(nuevo);
                this.Context.SaveChanges();
                return nuevo.id;
            }
        }


        public Comprador findByNumber(string number)
        {
            return this.Context.Compradores.FirstOrDefault(n => n.phoneNumber == number);
        }



    }
}
