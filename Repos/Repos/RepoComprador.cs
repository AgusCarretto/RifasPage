using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaAccesoDatos.Repos
{
    public class RepoComprador
    {

        public ProyectoContext Context { get; set; }
        public RepositorioEmpleado()
        {
            this.Context = new ProyectoContext();
        }







    }
}
