using Producto.Capa.BLL;
using Producto.Capa.DAL;
using Producto.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Producto.Controllers
{
    public class CompradorController : Controller
    {


        //Instanciamos a la capa de negocios
        private Negocios _productoBLL = new Negocios();


       

        // Vista principal del index, donde se muestran los articulos y 3 propiedades
        public ActionResult Index()
        {
            var productos = _productoBLL.ObtenerTodos();
            return View(productos);
        }


        // VerDetalles
        public ActionResult VerDetalles(int id)
        {
            var producto = _productoBLL.ObtenerProductoPorId(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }


        



        




    }
}