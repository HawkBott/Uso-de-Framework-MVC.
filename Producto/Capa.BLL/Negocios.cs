using Producto.Capa.DAL;
using Producto.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Producto.Capa.BLL
{
    public class Negocios
    {

        //Instanciamos a la capa DAL
        Datos datos = new Datos();






        //----------------------ALL LOGIN---------------------------------//

        //Metodo para autenticar al usuario por medio del email y password 
        public ResultadoAutenticacion AutenticarUsuario(string myEmail, string myPassword)
        {
            // Llamada al método AutenticarUsuario de la capa DAL
            // Ahora devuelve un objeto ResultadoAutenticacion
            var resultado = datos.AutenticarUsuario(myEmail, myPassword);
            return resultado;
        }


        //Metodo para obtener el id del usuario 
        public int ObtenerIdUsuario(string email)
        {
            try
            {
                return datos.ObtenerIdUsuario(email);
            }
            catch (Exception ex)
            {
                // Aquí puedes manejar la excepción como mejor te parezca
                throw new Exception("Error al obtener el ID del usuario: " + ex.Message);
            }
        }










        //----------------------ALL ADMINISTRATOR------------------------//


        // Metodo para dar de alta un producto 
        public void AltaProducto(string nombre, int cantidad, string descripcion, decimal precio, string foto)
        {
            try
            {
                datos.InsertarProducto(nombre, cantidad, descripcion, precio, foto);
            }
            catch (Exception ex)
            {
                // Aquí puedes manejar cualquier error que ocurra durante la inserción del producto
                throw new Exception("Error al insertar el producto: " + ex.Message);
            }
        }


        //Metodo para modificar un producto
        public bool ModificarProducto(int id, string nombre, int cantidad, string descripcion, decimal precio, string foto)
        {
            return datos.ActualizarProducto(id, nombre, cantidad, descripcion, precio, foto);
        }

        //Listado de productos
        public List<Producto.Entities.Producto> ListarProductos()
        {
            DataTable dt = datos.ObtenerProductos();
            List<Producto.Entities.Producto> productos = ConvertirAListaDeProductos(dt);
            return productos;
        }

        //Complemento para el listado de los productos
        private List<Producto.Entities.Producto> ConvertirAListaDeProductos(DataTable dt)
        {
            List<Producto.Entities.Producto> productos = new List<Producto.Entities.Producto>();

            foreach (DataRow row in dt.Rows)
            {
                Producto.Entities.Producto producto = new Producto.Entities.Producto
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Nombre = row["Nombre"].ToString(),
                    Precio = Convert.ToDecimal(row["Precio"])
                };

                productos.Add(producto);
            }

            return productos;
        }







        //----------------------ALL COMPRADOR------------------------//

        //Listado de detalles productos

        public List<Entities.Producto> ListarDetallesProductos()
        {
            DataTable dt = datos.ObtenerDetallesProductos();
            List<Producto.Entities.Producto> productos = ConvertirAListaDeProductosDetalles(dt);
            return productos;
        }

        private List<Entities.Producto> ConvertirAListaDeProductosDetalles(DataTable dt)
        {
            List<Producto.Entities.Producto> productos = new List<Producto.Entities.Producto>();

            foreach (DataRow row in dt.Rows)
            {
                Producto.Entities.Producto producto = new Producto.Entities.Producto
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Nombre = row["Nombre"].ToString(),
                    Cantidad = Convert.ToInt32(row["Cantidad"]),
                    Descripcion = row["Descripcion"].ToString(),
                    Precio = Convert.ToDecimal(row["Precio"]),
                    Foto = row["Foto"].ToString()
                };

                productos.Add(producto);
            }

            return productos;
        }









        //----------------------ALL CARRITO------------------------//

        //Metodo para agregar un producto al carrito tomando como referencia el id del producto y el id del comprador
        public void AgregarProductoAlCarrito(int usuarioId, int productoId, int cantidad)
        {
            // Aquí puedes agregar cualquier lógica de negocio adicional necesaria
            // antes de interactuar con la base de datos.

            datos.AgregarProductoAlCarrito(usuarioId, productoId, cantidad);
        }


        public void RealizarCompra(int usuarioId)
        {
            // Aquí puedes agregar cualquier lógica de negocio adicional necesaria
            // antes de interactuar con la base de datos.

            datos.RealizarCompra(usuarioId);
        }



        public void CancelarCompra(int usuarioId)
        {
            datos.CancelarCompra(usuarioId);
        }




        //Metodo para obtener los detatalles del producto que se visualizara antes de confirmar la compra
        public DataTable ObtenerDetallesDelProductoEnCarrito(int usuarioId)
        {
            // Llama al método de la capa de datos para obtener los detalles del producto
            DataTable dt = datos.ObtenerDetallesDelProductoEnCarrito(usuarioId);

            // Aquí puedes agregar cualquier lógica de negocio adicional que necesites

            return dt;
        }



        //Metodo para calcular el total de lo que tiene que pagar el usuario
        public DataTable GetTotalPorUsuario(int usuarioId)
        {
            return datos.GetTotalPorUsuario(usuarioId);
        }


        //Metodo para obtener el carrito por le id del usuario
        public int ObtenerCarritoId(int usuarioId)
        {
            try
            {
                return datos.ObtenerCarritoId(usuarioId);
            }
            catch (Exception ex)
            {
                // Aquí puedes manejar la excepción como mejor te parezca
                throw new Exception("Error al obtener el ID del carrito: " + ex.Message);
            }
        }





























        public Entities.Producto ObtenerProductoPorId(int id)
        {
            return datos.ObtenerPorId(id);
        }













        public DataTable ObtenerUsuarios()
        {
            return datos.GetUsuarios();
        }



        public List<Entities.Producto> ObtenerTodos()
        {
            return datos.ObtenerTodos();
        }









        



        public List<Entities.Producto> ObtenerProductosCarrito(int usuarioId)
        {
            return datos.ObtenerProductosCarrito(usuarioId);
        }


    }
}