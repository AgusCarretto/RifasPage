using LogicaAccesoDatos;
using LogicaAccesoDatos.Repositorios;
using LogicaNegocio;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Diagnostics;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;

        private RepositorioRifa _repoRifa;
        private RepositorioComprador _repoComprador;
        private ServicioEmail _emailService;


        public HomeController(RepositorioRifa repoRifa, RepositorioComprador repoComprador, ServicioEmail emailService)
        {

            _repoRifa = repoRifa;
            _repoComprador = repoComprador;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            var rifas = _repoRifa.ObtenerTodas();

            int total = rifas.Count();
            int vendidas = rifas.Count(r => r.state == Rifa.EstadoRifa.Vendido || r.state == Rifa.EstadoRifa.Reservado);

            var mejorComprador = _repoRifa.ObtenerMejorComprador();

            //En caso que sea nullo manda el se el primero o el 0 para que se sepa que no hay comprador
            ViewBag.MejorComprador = mejorComprador?.Nombre ?? "¡Sé el primero!";
            ViewBag.MejorCantidad = mejorComprador?.Cantidad ?? 0;
            ViewBag.Porcentaje = (total > 0) ? (vendidas * 100 / total) : 0;
            ViewBag.RifasRestantes = total - vendidas;


            return View(rifas);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }






        [HttpPost]
        public IActionResult Reservar(string idRifas, string nombre, string telefono, string email)
        {

            try
            {
                var nuevoComprador = new Comprador
                {
                    name = nombre,
                    phoneNumber = telefono,
                    email = email
                };

                int idNuevoComprador = _repoComprador.Agregar(nuevoComprador);
                _repoRifa.ReservarVariasRifas(idRifas, idNuevoComprador);

                decimal montoTotal = _repoRifa.calcularMonto(idRifas);


                try
                {
                    string mensajeWpp = $"¡Hola Agustín! 👋 Aquí envío el comprobante por las rifas: {idRifas}";
                    string mensajeWppUrl = System.Net.WebUtility.UrlEncode(mensajeWpp);
                    string linkWpp = $"https://wa.me/59892114480?text={mensajeWppUrl}";

                    /*Cuerpo del mail a enviar al comprador*/

                    string cuerpoMail = $@"
                        <div style='font-family: Arial; border: 1px solid #2e7d32; padding: 20px;'>
                            <h2 style='color: #2e7d32;'>LONDRES 2027 - Reserva Exitosa</h2>
                            <p>Hola <b>{nombre}</b>, muchisimas gracias por ayudarme a hacer este viaje realidad!</p>
                            <p><b>Reservaste los números: {idRifas}</b></p>
                            <p><b>Total a transferir: ${montoTotal}</b></p>
                            <hr>
                            <p><b>Cuenta ITAU:</b> 3901430</p>
                            <p><b>Titular:</b> Agustín Carretto</p>

                            <div style='margin-top: 25px; text-align: center;'>
                                <p style='color: #666; font-size: 0.9em;'>Una vez realizada la transferencia, hacé clic debajo:</p>
                                 <a href='{linkWpp}' 
                                   style='background-color: #25D366; color: white; padding: 12px 25px; text-decoration: none; border-radius: 50px; font-weight: bold; display: inline-block;'>
                                   ✅ ENVIAR COMPROBANTE POR WHATSAPP
                                 </a>
                            </div>
                    

                        </div>";


                    _emailService.AlertaDeReserva(nombre, idRifas, email, telefono);
                    _emailService.EnviarEmail(email, "Tu reserva de rifas", cuerpoMail);
                }
                catch (Exception ex)
                {

                }

                return Json(new { success = true, ids = idRifas, total = montoTotal });

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                var rifas = _repoRifa.ObtenerTodas();

                return Json(new { success = false, message = ex.Message });
            }

        }



    }
}
