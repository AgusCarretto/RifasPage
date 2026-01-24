using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LogicaAccesoDatos
{
    public class ServicioEmail
    {


        private readonly IConfiguration _config;

        public ServicioEmail(IConfiguration config)
        {
            _config = config;
        }

        public void AlertaDeReserva(string nombre, string rifaId, string emailComprador, string telefono)
        {


            var user = _config["EmailSettingsAdmin:SenderEmail"];
            var passAdmin = _config["EmailSettingsAdmin:AppPassword"];
            var fromAddress = new MailAddress(user, "Sistema de Rifas");
            var toAddress = new MailAddress(user);
            var port = int.Parse(_config["EmailSettingsAdmin:Port"]);

            const string subject = "⚠️ ¡Nueva Reserva de Rifa!";
            string body = $"Hola Agus,\n\nAlguien ha reservado la rifa #{rifaId}.\nNombre: {nombre}\nTelefono:{telefono} \nEmail: {emailComprador}\n\nRevisa el panel de control para confirmar el pago.";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, passAdmin)
            };

            using (var message = new MailMessage(fromAddress, toAddress) { Subject = subject, Body = body })
            {
                smtp.Send(message);
            }
        }



        public void EnviarEmail(string emailDestino, string asunto, string cuerpoHtml)
        {
            var host = _config["EmailSettings:SmtpServer"];
            var port = int.Parse(_config["EmailSettings:Port"]);
            var user = _config["EmailSettings:SenderEmail"];
            var pass = _config["EmailSettings:AppPassword"];

            var smtpClient = new SmtpClient("smtp.gmail.com") // Servidor de Gmail
            {
                Port = port,
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = true,
            };


            var mailMessage = new MailMessage
            {
                From = new MailAddress(user, _config["EmailSettings:SenderName"]),
                Subject = asunto,
                Body = cuerpoHtml,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(emailDestino);

            smtpClient.Send(mailMessage);

        }





    }
}
