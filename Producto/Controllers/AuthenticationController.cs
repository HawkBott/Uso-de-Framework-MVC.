using Producto.Capa.BLL;
using Producto.Capa.DAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Producto.Controllers
{
    public class AuthenticationController : Controller
    {


        Negocios negocios = new Negocios();


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Login(string myEmail, string myPassword)
        {
            // Ahora esta llamada devuelve un objeto ResultadoAutenticacion
            var resultado = negocios.AutenticarUsuario(myEmail, myPassword);

            if (resultado.IsAuthenticated)
            {
                // Imprimir el correo electrónico del usuario en la consola
                System.Diagnostics.Debug.WriteLine("Correo electrónico del usuario: " + myEmail);

                // Obtener el ID del usuario
                int idUsuario = negocios.ObtenerIdUsuario(myEmail);
                Session["UsuarioId"] = idUsuario; // Almacenar el ID del usuario en una variable de sesión

                // Obtener el ID del carrito para este usuario
                int carritoId = negocios.ObtenerCarritoId(idUsuario);
                Session["CarritoId"] = carritoId; // Almacenar el ID del carrito en una variable de sesión

                // Utiliza el rol del usuario para redirigirlo a diferentes vistas
                switch (resultado.Role)
                {
                    case "Administrador":
                        return RedirectToAction("Index", "Administrator");

                    case "Comprador":
                        return RedirectToAction("Index", "Comprador");

                    default:
                        return RedirectToAction("Login", "Authentication");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Correo electrónico o contraseña incorrectos");
                return View();
            }
        }




        




    }

}