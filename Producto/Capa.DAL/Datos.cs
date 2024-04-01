using Producto.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Producto.Capa.DAL
{
    

    public class Datos
    {
        private SqlConnection con;

        // Constructor para inicializar la conexión
        public Datos()
        {
            // Asegúrate de que la cadena de conexión "miConexion" está definida en tu archivo de configuración
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["miConexion"].ConnectionString);
        }

        string connectionString = ConfigurationManager.ConnectionStrings["miConexion"].ConnectionString;





        //----------------------ALL LOGIN---------------------------------//


        public static Dictionary<string, Usuario> Sesiones = new Dictionary<string, Usuario>();

        // Método para autenticar usuario
        public ResultadoAutenticacion AutenticarUsuario(string myEmail, string myPassword)
        {
            ResultadoAutenticacion resultado = new ResultadoAutenticacion();

            // Consulta para obtener el usuario y su RolId
            string queryUsuario = "SELECT Id, RolId FROM Usuarios WHERE MyEmail = @CorreoElectronico AND MyPassword = @Contraseña";

            using (SqlCommand cmdUsuario = new SqlCommand(queryUsuario, con))
            {
                cmdUsuario.Parameters.AddWithValue("@CorreoElectronico", myEmail);
                cmdUsuario.Parameters.AddWithValue("@Contraseña", myPassword);

                try
                {
                    con.Open();
                    using (var reader = cmdUsuario.ExecuteReader())
                    {
                        if (reader.Read()) // Si hay al menos un resultado, el usuario está autenticado
                        {
                            resultado.IsAuthenticated = true;
                            int usuarioId = (int)reader["Id"]; // Obtiene el Id del usuario
                            int rolId = (int)reader["RolId"];
                            // Ahora obtenemos el nombre del rol con el RolId
                            reader.Close(); // Es importante cerrar el reader antes de ejecutar otro comando

                            string queryRol = "SELECT NombreRol FROM Roles WHERE RolId = @RolId";
                            using (SqlCommand cmdRol = new SqlCommand(queryRol, con))
                            {
                                cmdRol.Parameters.AddWithValue("@RolId", rolId);
                                var rolName = cmdRol.ExecuteScalar();
                                resultado.Role = rolName != null ? rolName.ToString() : null;
                            }

                            // Agregar la sesión del usuario al diccionario
                            var sesionUsuario = new Usuario
                            {
                                Id = usuarioId, // Guarda el Id del usuario aquí
                                CarritoId = ObtenerCarritoId(usuarioId), // Aquí obtienes y guardas el CarritoId del usuario
                                MyEmail = myEmail,
                                Rol = new Rol { RolId = rolId, NombreRol = resultado.Role }
                            };
                            Sesiones.Add(myEmail, sesionUsuario);

                            // Imprimir el correo electrónico del usuario para verificar que se ha guardado correctamente
                            System.Diagnostics.Debug.WriteLine("Correo electrónico del usuario guardado: " + Sesiones[myEmail].MyEmail);
                        }

                        else
                        {
                            resultado.IsAuthenticated = false;
                            resultado.Role = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de la excepción
                    /*Console.WriteLine(ex.Message);*/
                    resultado.IsAuthenticated = false;
                    resultado.Role = null;
                }
                finally
                {
                    con.Close();
                }
            }

            return resultado;
        }



        //Metodo para obtener el ID del usuario por medio del email
        public int ObtenerIdUsuario(string myEmail)
        {
            if (Sesiones.ContainsKey(myEmail))
            {
                return Sesiones[myEmail].Id;
            }
            else
            {
                throw new Exception("El usuario no está autenticado o la sesión no existe.");
            }
        }

        



        public int ObtenerCarritoId(int usuarioId)
        {
            int carritoId = 0;

            // Consulta para obtener el CarritoId del usuario
            string queryCarrito = "SELECT CarritoId FROM Carrito WHERE UsuarioId = @UsuarioId";

            using (SqlCommand cmdCarrito = new SqlCommand(queryCarrito, con))
            {
                cmdCarrito.Parameters.AddWithValue("@UsuarioId", usuarioId);

                try
                {
                    con.Open();
                    using (var reader = cmdCarrito.ExecuteReader())
                    {
                        if (reader.Read()) // Si hay al menos un resultado, el usuario tiene un carrito
                        {
                            carritoId = (int)reader["CarritoId"]; // Obtiene el CarritoId del usuario
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de la excepción
                    /*Console.WriteLine(ex.Message);*/
                }
                finally
                {
                    con.Close();
                }
            }

            return carritoId;
        }





        












        //----------------------ALL ADMINISTRATOR------------------------//


        //Alta de un producto
        public void InsertarProducto(string nombre, int cantidad, string descripcion, decimal precio, string foto)
        {
            string query = "INSERT INTO Productos (Nombre, Cantidad, Descripcion, Precio, Foto) VALUES (@Nombre, @Cantidad, @Descripcion, @Precio, @Foto)";

            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@Nombre", nombre);
                command.Parameters.AddWithValue("@Cantidad", cantidad);
                command.Parameters.AddWithValue("@Descripcion", descripcion);
                command.Parameters.AddWithValue("@Precio", precio);
                command.Parameters.AddWithValue("@Foto", foto);

                con.Open();
                command.ExecuteNonQuery();
                con.Close();
            }
        }


        //Metodo para modificar un producto
        public bool ActualizarProducto(int id, string nombre, int cantidad, string descripcion, decimal precio, string foto)
        {
            string query = "UPDATE Productos SET Nombre = @Nombre, Cantidad = @Cantidad, Descripcion = @Descripcion, Precio = @Precio, Foto = @Foto WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                cmd.Parameters.AddWithValue("@Precio", precio);
                cmd.Parameters.AddWithValue("@Foto", foto);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                return rowsAffected > 0;
            }
        }









        //----------------------ALL COMPRADOR------------------------//


        //Listado de detalles productos

        public DataTable ObtenerDetallesProductos()
        {
            DataTable dt = new DataTable();
            string query = "SELECT * FROM Productos";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }




        //----------------------ALL CARRITO------------------------//

        //Metodo para agregar un producto al carrito tomando como referencia el id del producto y el id del comprador
        public void AgregarProductoAlCarrito(int usuarioId, int productoId, int cantidad)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Comprueba si el usuario ya tiene un carrito
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Carrito WHERE UsuarioId = @UsuarioId", con);
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                int count = (int)cmd.ExecuteScalar();

                // Si el usuario no tiene un carrito, crea uno
                if (count == 0)
                {
                    cmd = new SqlCommand("INSERT INTO Carrito (UsuarioId) VALUES (@UsuarioId)", con);
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    cmd.ExecuteNonQuery();
                }

                // Obtiene el ID del carrito del usuario
                cmd = new SqlCommand("SELECT CarritoId FROM Carrito WHERE UsuarioId = @UsuarioId", con);
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                int carritoId = (int)cmd.ExecuteScalar();

                // Agrega el producto al carrito
                cmd = new SqlCommand("INSERT INTO CarritoProductos (CarritoId, ProductoId, Cantidad) VALUES (@CarritoId, @ProductoId, @Cantidad)", con);
                cmd.Parameters.AddWithValue("@CarritoId", carritoId);
                cmd.Parameters.AddWithValue("@ProductoId", productoId);
                cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                cmd.ExecuteNonQuery();

                con.Close();
            }
        }

        public void RealizarCompra(int usuarioId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Obtiene el ID del carrito del usuario
                SqlCommand cmd = new SqlCommand("SELECT CarritoId FROM Carrito WHERE UsuarioId = @UsuarioId", con);
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                int carritoId = (int)cmd.ExecuteScalar();

                // Realiza la compra
                cmd = new SqlCommand("EXEC RealizarCompra @UsuarioId", con);
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                cmd.ExecuteNonQuery();

                con.Close();
            }
        }


        public void CancelarCompra(int usuarioId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Comprueba si el usuario ya tiene un carrito
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Carrito WHERE UsuarioId = @UsuarioId", con);
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                int count = (int)cmd.ExecuteScalar();

                // Si el usuario tiene un carrito, cancela la compra
                if (count > 0)
                {
                    // Obtiene el ID del carrito del usuario
                    cmd = new SqlCommand("SELECT CarritoId FROM Carrito WHERE UsuarioId = @UsuarioId", con);
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    int carritoId = (int)cmd.ExecuteScalar();

                    // Ejecuta el procedimiento almacenado para cancelar la compra
                    cmd = new SqlCommand("EXEC CancelarCompra @UsuarioId", con);
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
        }



        //Metodo para obtener los detatalles del producto que se visualizara antes de confirmar la compra
        public DataTable ObtenerDetallesDelProductoEnCarrito(int usuarioId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT p.Id, p.Nombre, cp.Cantidad, p.Descripcion, p.Precio, p.Foto FROM Carrito c INNER JOIN CarritoProductos cp ON c.CarritoId = cp.CarritoId INNER JOIN Productos p ON cp.ProductoId = p.Id WHERE c.UsuarioId = @UsuarioId", con);
            cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }




        public DataTable GetTotalPorUsuario(int usuarioId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT Usuarios.NombreCompleto, SUM(CarritoProductos.Cantidad * Productos.Precio) AS Total FROM Carrito JOIN Usuarios ON Carrito.UsuarioId = Usuarios.Id JOIN CarritoProductos ON Carrito.CarritoId = CarritoProductos.CarritoId JOIN Productos ON CarritoProductos.ProductoId = Productos.Id WHERE Usuarios.Id = @UsuarioId GROUP BY Usuarios.NombreCompleto", con))
                {
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    return dt;
                }
            }
        }












































        //Listado de productos
        public DataTable ObtenerProductos()
        {
            DataTable dt = new DataTable();
            string query = "SELECT Id, Nombre, Precio FROM Productos";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        


        


   






        public DataTable GetUsuarios()
        {
            DataTable dt = new DataTable();
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Usuarios", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los usuarios: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
            return dt;
        }



        public List<Entities.Producto> ObtenerTodos()
        {
            List<Entities.Producto> productos = new List<Entities.Producto>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT Id, Nombre, Cantidad, Descripcion, Precio, Foto FROM Productos", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    productos.Add(new Entities.Producto
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nombre = reader["Nombre"].ToString(),
                        Cantidad = Convert.ToInt32(reader["Cantidad"]),
                        Descripcion = reader["Descripcion"].ToString(),
                        Precio = Convert.ToDecimal(reader["Precio"]),
                        Foto = reader["Foto"].ToString()
                    });
                }

                reader.Close();
                connection.Close();
            }

            return productos;
        }


        public Entities.Producto ObtenerPorId(int id)
        {
            Entities.Producto producto;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT Id, Nombre, Cantidad, Descripcion, Precio, Foto FROM Productos WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    producto = new Entities.Producto
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nombre = reader["Nombre"].ToString(),
                        Cantidad = Convert.ToInt32(reader["Cantidad"]),
                        Descripcion = reader["Descripcion"].ToString(),
                        Precio = Convert.ToDecimal(reader["Precio"]),
                        Foto = reader["Foto"].ToString()
                    };
                }
                else
                {
                    producto = null;
                }

                reader.Close();
                connection.Close();
            }

            return producto;
        }






        





        public List<Entities.Producto> ObtenerProductosCarrito(int usuarioId)
        {
            List<Entities.Producto> productosCarrito = new List<Entities.Producto>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT p.Id, p.Nombre, cp.Cantidad, p.Descripcion, p.Precio, p.Foto " +
                               "FROM Carrito c " +
                               "JOIN CarritoProductos cp ON c.CarritoId = cp.CarritoId " +
                               "JOIN Productos p ON cp.ProductoId = p.Id " +
                               "WHERE c.UsuarioId = @UsuarioId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Entities.Producto producto = new Entities.Producto
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Cantidad = reader.GetInt32(2),
                                Descripcion = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Precio = reader.GetDecimal(4),
                                Foto = reader.IsDBNull(5) ? null : reader.GetString(5)
                            };

                            productosCarrito.Add(producto);
                        }
                    }
                }
            }

            return productosCarrito;
        }






    }



}