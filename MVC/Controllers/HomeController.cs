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
        public IActionResult Reservar (int idRifa, string nombre, string telefono, string email)
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
            catch
            {
                // Si el mail falla, que la app no se rompa, la reserva ya se hizo
            }


            return RedirectToAction("Index");


        }





    }
}
