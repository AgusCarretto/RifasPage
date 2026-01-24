using LogicaNegocio.Interfaces;
using System.Collections;
using System.Text.RegularExpressions;

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
            validarMail();
            validarNumero();
        }


        public void validarMail()
        {
            string caracteres = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            //Esto es para que no venga nullo ni con espacios
            if (string.IsNullOrWhiteSpace(this.email))
            {
                throw new Exception("Ingrese un mail valido");
            }

            //Famoso Regex
            if (!Regex.IsMatch(this.email, caracteres))
            {
                throw new Exception("Ingrese un mail valido");
            }

        }



        public void validarNumero()
        {
            if (string.IsNullOrWhiteSpace(this.phoneNumber)) //nullo o con espacios negativo central
                throw new Exception("El teléfono es obligatorio.");

            
            string motivo = @"^[0-9]{8,11}$";

            if (!Regex.IsMatch(this.phoneNumber, motivo))
            {
                throw new Exception("El formato del teléfono no es válido. Ingresa solo números.");
            }
        }


    }
}
