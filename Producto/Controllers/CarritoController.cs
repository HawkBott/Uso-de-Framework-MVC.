using Producto.Capa.BLL;
using Producto.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Producto.Controllers
{
    public class CarritoController : Controller
    {
        //Instanciamos a la capa de negocios
        private Negocios _productoBLL = new Negocios();


        // AgregarAlCarrito
        [HttpPost]
        public ActionResult AgregarAlCarrito(int productoId, int cantidad)
        {
            if (Session["UsuarioId"] == null)
            {
                // Redirige al usuario a la página de inicio de sesión si la sesión ha expirado
                return RedirectToAction("Login", "Authentication");
            }

            int usuarioId = (int)Session["UsuarioId"];
            _productoBLL.AgregarAlCarrito(usuarioId, productoId, cantidad);
            return RedirectToAction("Index", "Comprador");
        }


        // method for see details of producto in carrito
        public ActionResult DetallesDelProductoEnCarrito()
        {
            if (Session["UsuarioId"] == null)
            {
                // Redirige al usuario a la página de inicio de sesión si la sesión ha expirado
                return RedirectToAction("Login", "Authentication");
            }

            int usuarioId = (int)Session["UsuarioId"];
            DataTable dt = _productoBLL.ObtenerDetallesDelProductoEnCarrito(usuarioId);
            DataTable dtTotal = _productoBLL.GetTotalPorUsuario(usuarioId);

            // Verificar si dtTotal tiene al menos una fila
            if (dtTotal.Rows.Count > 0)
            {
                // Verificar si el total es null o cero
                if (dtTotal.Rows[0]["Total"] == null || Convert.ToDecimal(dtTotal.Rows[0]["Total"]) == 0)
                {
                    ViewBag.Total = "No hay ningún producto en el carrito";
                }
                else
                {
                    ViewBag.Total = dtTotal.Rows[0]["Total"].ToString();
                }
            }
            else
            {
                ViewBag.Total = "No hay ningún producto en el carrito";
            }

            return View(dt);
        }





        [HttpPost]
        public ActionResult RealizarCompra()
        {
            try
            {
                // Obtén el CarritoId del usuario actual
                int carritoId = (int)Session["CarritoId"];

                // Registra la compra
                _productoBLL.RegistrarCompra(carritoId);

                // Retorna una vista de éxito en un controlador específico
                return RedirectToAction("Index", "Comprador");
            }
            catch (NullReferenceException ex)
            {
                // Captura excepciones para referencias nulas, como un Session["CarritoId"] que podría no estar definido
                Console.WriteLine($"Error de referencia nula: {ex.Message}");
                return View("DetallesDelProductoEnCarrito"); // Retorna una vista de error genérica
            }
            catch (InvalidOperationException ex)
            {
                // Captura excepciones para operaciones inválidas, como intentar registrar una compra con un ID inválido
                Console.WriteLine($"Operación inválida: {ex.Message}");
                return View("DetallesDelProductoEnCarrito"); // Retorna una vista de error genérica
            }
            catch (SqlException ex)
            {
                // Captura excepciones relacionadas con la base de datos
                Console.WriteLine($"Error de base de datos: {ex.Message}");
                return View("DetallesDelProductoEnCarrito"); // Retorna una vista de error genérica
            }
            catch (Exception ex)
            {
                // Captura cualquier otra excepción no manejada específicamente
                Console.WriteLine($"Excepción no manejada: {ex.Message}");
                return View("DetallesDelProductoEnCarrito"); // Retorna una vista de error genérica
            }
        }



        [HttpPost]
        public ActionResult CancelarCompra(int usuarioId)
        {
            try
            {
                _productoBLL.CancelarCompra(usuarioId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción no manejada: {ex.Message}");
                return View("DetallesDelProductoEnCarrito"); // Retorna una vista de error genérica
            }
        }






    }
}