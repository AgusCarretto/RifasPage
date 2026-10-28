using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LogicaAccesoDatos
{
    /// <summary>
    /// Envía mails por la API HTTPS de Brevo (https://developers.brevo.com/reference/sendtransacemail).
    /// No usa SMTP porque Railway bloquea los puertos SMTP salientes en el plan Hobby.
    /// Configuración: EmailSettings:ApiKey, EmailSettings:SenderEmail, EmailSettings:SenderName y EmailSettings:AdminEmail.
    /// </summary>
    public class ServicioEmail
    {
        private const string Endpoint = "https://api.brevo.com/v3/smtp/email";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private readonly IConfiguration _config;
        private readonly HttpClient _http;
        private readonly ILogger<ServicioEmail> _logger;

        public ServicioEmail(IConfiguration config, HttpClient http, ILogger<ServicioEmail> logger)
        {
            _config = config;
            _http = http;
            _logger = logger;
        }

        /// <summary>El envío es opcional: sin EmailSettings:ApiKey la app funciona igual y simplemente no manda mails.</summary>
        public bool Habilitado => !string.IsNullOrWhiteSpace(_config["EmailSettings:ApiKey"]);

        // Son async para que cualquier error (incluida la configuración faltante) llegue como Task fallida y no como excepción síncrona.
        public async Task AlertaDeReserva(string nombre, string rifaId, string emailComprador, string telefono)
        {
            const string subject = "⚠️ ¡Nueva Reserva de Rifa!";
            if (!Habilitado)
            {
                AvisarDeshabilitado(subject);
                return;
            }

            var admin = Requerida("EmailSettings:AdminEmail");

            string body = $"Hola Agus,\n\nAlguien ha reservado la rifa #{rifaId}.\nNombre: {nombre}\nTelefono:{telefono} \nEmail: {emailComprador}\n\nRevisa el panel de control para confirmar el pago.";

            await Enviar(admin, subject, htmlContent: null, textContent: body, nombreRemitente: "Sistema de Rifas");
        }

        public async Task EnviarEmail(string emailDestino, string asunto, string cuerpoHtml)
        {
            if (!Habilitado)
            {
                AvisarDeshabilitado(asunto);
                return;
            }

            await Enviar(emailDestino, asunto, htmlContent: cuerpoHtml, textContent: null, nombreRemitente: _config["EmailSettings:SenderName"]);
        }

        private async Task Enviar(string destino, string asunto, string? htmlContent, string? textContent, string? nombreRemitente)
        {
            var apiKey = Requerida("EmailSettings:ApiKey");
            var remitente = Requerida("EmailSettings:SenderEmail");

            var payload = new
            {
                sender = new { name = nombreRemitente, email = remitente },
                to = new[] { new { email = destino } },
                subject = asunto,
                htmlContent,
                textContent
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint)
            {
                Content = JsonContent.Create(payload, options: JsonOptions)
            };
            request.Headers.Add("api-key", apiKey);

            using var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var detalle = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Brevo respondió {(int)response.StatusCode}: {detalle}");
            }
        }

        private void AvisarDeshabilitado(string asunto)
        {
            _logger.LogInformation("Mail '{Asunto}' no enviado: falta EmailSettings:ApiKey (envío de mails deshabilitado).", asunto);
        }

        private string Requerida(string clave)
        {
            var valor = _config[clave];
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new InvalidOperationException($"Falta la configuración '{clave}'.");
            }
            return valor;
        }
    }
}
