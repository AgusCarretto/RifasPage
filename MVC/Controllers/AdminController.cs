using LogicaAccesoDatos.Repositorios;
using LogicaNegocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MVC.Controllers
{
    public class AdminController : Controller
    {
        // GET: AdminController

        private readonly ILogger<HomeController> _logger;

        private RepositorioRifa _repoRifa;
        private RepositorioComprador _repoComprador;
        private RepositorioAdmin _repoAdmin;
        public AdminController(RepositorioRifa repoRifa, RepositorioComprador repoComprador, RepositorioAdmin repoAdmin)
        {
            _repoAdmin = repoAdmin;
            _repoRifa = repoRifa;
            _repoComprador = repoComprador;
        }


        [HttpGet]
        public ActionResult Login()
        {
            try
            {
                if (HttpContext.Session.GetString("admin") != null)
                {
                    return RedirectToAction("Dashboard");
                }
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }



        }

        [HttpPost]
        public ActionResult Login(string name, string pass)
        {
            try
            {
                if (_repoAdmin.loginUsuario(name, pass))
                {
                    HttpContext.Session.SetString("admin", name);
                    return RedirectToAction("Dashboard");
                }
                ViewBag.Error = "Nombre de usuario o contraseña incorrectas.";
                return View();
            }

            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error durante el inicio de sesión.";
                return View();
            }


        }


        public ActionResult Dashboard(Admin ad)
        {
            try
            {
                if(HttpContext.Session.GetString("admin") == null)
                {
                    return RedirectToAction("Login");
                }
                else{

                    var rifasVendidas = _repoRifa.ContarRifasVendidas();
                    var rifasReservadas = _repoRifa.CantidadDeRifasReservadas();
                    var totalRecaudado = _repoRifa.CalcularRecaudacionTotal();

                    ViewBag.Vendidas = rifasVendidas;
                    ViewBag.Reservadas = rifasReservadas;
                    ViewBag.Recaudacion = totalRecaudado;
                    ViewBag.Progreso = (ViewBag.Vendidas * 100) / 80;

                    var rifas = _repoRifa.ObtenerTodas();
                    return View(rifas);

                }

            }catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error al cargar el panel de administración.";
                return View("Login");
            }
        }

        [HttpPost]
        public IActionResult ConfirmarPago(int idRifa)
        {

            if (HttpContext.Session.GetString("admin") != null)
            {
                _repoRifa.ActualizarEstado(idRifa, Rifa.EstadoRifa.Vendido);

                return RedirectToAction("Dashboard");
            }
            else
            {
                HttpContext.Session.Remove("admin");
                return RedirectToAction("Login");
            }
                
        }


        [HttpPost]
        public async Task<IActionResult> CancelarRifa(int id)
        {
            await _repoRifa.CancelarRifa(id);
            TempData["SuccessMessage"] = $"La rifa {id} esta disponible nuevamente.";
            return RedirectToAction("Dashboard");

        }


        public IActionResult Logout()
        {
            HttpContext.Session.Remove("admin");
            return RedirectToAction("Login");
        }




        [HttpPost]
        public async Task<IActionResult> ResetearRifas()
        {
            await _repoRifa.cargaRifas80();
            return RedirectToAction("Dashboard");
        }



    }
}
