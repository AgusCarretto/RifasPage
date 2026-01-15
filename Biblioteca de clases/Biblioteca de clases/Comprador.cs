using LogicaNegocio.Interfaces;
using System.Collections;

namespace LogicaNegocio
{
    public class Comprador : IValidable
    {
        public int id { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
      
        public Comprador() { }
        public Comprador(string name, string phoneNumber, string email)
        {
            this.name = name;
            this.phoneNumber = phoneNumber;
            this.email = email;
        }

        public void Validar()
        {
            throw new NotImplementedException();
        }
    }
}
