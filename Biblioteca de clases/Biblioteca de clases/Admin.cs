using LogicaNegocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio
{
    public class Admin : IValidable
    {
        public int id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string email { get; set; }

        public Admin() { }
        public Admin(string name, string password, string email)
        {
            this.name = name;
            this.password = password;
            this.email = email;
        }


        public void Validar()
        {
         //El admin se precarga y soy solo yo.   
        }
    }
}
