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
    public class RepositorioAdmin
    {

        public ProyectoContext Context { get; set; }
        public RepositorioAdmin(ProyectoContext context)
        {
            this.Context = context;
        }

       
        public bool loginUsuario(string name, string pass)
        {
            return Context.Admin.Any(a => a.name == name && a.password == pass);

        }


}
}
