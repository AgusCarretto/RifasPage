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
        public HomeController(RepositorioRifa repoRifa, RepositorioComprador repoComprador)
        {

            _repoRifa = repoRifa;
            _repoComprador = repoComprador;
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
        public IActionResult Reservar(int idRifa, string nombre, string telefono, string email)
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
                _repoRifa.ReservarRifa(idRifa, idNuevoComprador);

                try
                {
                    var emailService = new ServicioEmail();
                    emailService.EnviarAlertaReserva(nombre, idRifa.ToString(), email, telefono);
                }
                catch (Exception ex) 
                {
                    
                }


                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                var rifas = _repoRifa.ObtenerTodas();

                return View("Index", rifas);
            }

        }





    }
}
