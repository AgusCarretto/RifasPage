using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LogicaAccesoDatos
{
    public class ServicioEmail
    {
        public void EnviarAlertaReserva(string nombre, string rifaId, string emailComprador, string telefono)
        {
            var fromAddress = new MailAddress("agus.carretto@gmail.com", "Sistema de Rifas");
            var toAddress = new MailAddress("agus.carretto@gmail.com");
            const string fromPassword = "ycjv uafp uwmy bsln"; // No es tu clave normal, es una especial de Google
            const string subject = "⚠️ ¡Nueva Reserva de Rifa!";
            string body = $"Hola Agus,\n\nAlguien ha reservado la rifa #{rifaId}.\nNombre: {nombre}\nTelefono:{telefono} \nEmail: {emailComprador}\n\nRevisa el panel de control para confirmar el pago.";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress) { Subject = subject, Body = body })
            {
                smtp.Send(message);
            }
        }
    }
}
