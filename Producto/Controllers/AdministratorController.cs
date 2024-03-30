using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Producto.Capa.BLL;
using Producto.Entities; // Si necesitas referenciar otros elementos del espacio de nombres Producto.Entities
using ProductoModel = Producto.Entities.Producto;

namespace Producto.Controllers
{


    public class AdministratorController : Controller
    {

        private Negocios _productoBLL = new Negocios();






        // GET: Administrator
        public ActionResult Index()
        {
            return View();
        }




        [HttpGet]
        public ActionResult AltaProducto()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AltaProducto(string nombre, int cantidad, string descripcion, decimal precio, string foto)
        {
            try
            {
                _productoBLL.AltaProducto(nombre, cantidad, descripcion, precio, foto);
                ViewBag.Message = "Producto agregado exitosamente.";
            }
            catch (Exception ex)
            {
                // Aquí puedes manejar cualquier error que ocurra durante la inserción del producto
                ViewBag.Message = "Error al agregar el producto: " + ex.Message;
            }

            // Redirige al usuario a la vista "ListarProductos.cshtml"
            return RedirectToAction("ListarProductos");
        }





        [HttpGet]
        public ActionResult ModificarProducto()
        {
            return View();
        }



        [HttpPost]
        public ActionResult ModificarProducto(int id, string nombre, int cantidad, string descripcion, decimal precio, string foto)
        {
            bool exito = _productoBLL.ModificarProducto(id, nombre, cantidad, descripcion, precio, foto);

            if (exito)
            {
                ViewBag.Message = "Producto actualizado con éxito.";
            }
            else
            {
                ViewBag.Message = "Error al actualizar el producto.";
            }

            // Aquí puedes redirigir a la vista que desees, por ejemplo, una vista de detalles del producto
            return View();
        }




        [HttpGet]
        public ActionResult ListarProductos()
        {
            var productos = _productoBLL.ListarProductos();
            return View(productos);
        }


        [HttpGet]
        public ActionResult ListarDetallesProductos()
        {
            var productos = _productoBLL.ListarDetallesProductos();
            return View(productos);
        }
        
    }


}

