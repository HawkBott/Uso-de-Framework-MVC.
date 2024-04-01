using Producto.Capa.BLL;
using Producto.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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

            _productoBLL.AgregarProductoAlCarrito(usuarioId, productoId, cantidad);

            // Redirige al usuario de vuelta a la página del carrito después de agregar el producto
            return RedirectToAction("DetallesDelProductoEnCarrito", "Carrito");
        }


        [HttpPost]
        public ActionResult RealizarCompra()
        {
            if (Session["UsuarioId"] == null)
            {
                // Redirige al usuario a la página de inicio de sesión si la sesión ha expirado
                return RedirectToAction("Login", "Authentication");
            }

            int usuarioId = (int)Session["UsuarioId"];

            _productoBLL.RealizarCompra(usuarioId);

            // Redirige al usuario de vuelta a la página del carrito después de realizar la compra
            return RedirectToAction("DetallesDelProductoEnCarrito", "Carrito");
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
        public ActionResult CancelarCompra()
        {
            if (Session["UsuarioId"] == null)
            {
                // Redirige al usuario a la página de inicio de sesión si la sesión ha expirado
                return RedirectToAction("Login", "Authentication");
            }

            int usuarioId = (int)Session["UsuarioId"];
            _productoBLL.CancelarCompra(usuarioId);

            return RedirectToAction("DetallesDelProductoEnCarrito", "Carrito");
        }










    }
}