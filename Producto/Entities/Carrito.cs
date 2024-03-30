using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Producto.Entities
{
    public class Carrito
    {
        public int CarritoId { get; set; } // Clave primaria, autoincremental
        public int UsuarioId { get; set; } // Clave foránea que referencia a la tabla Usuarios
        public int ProductoId { get; set; } // Clave foránea que referencia a la tabla Productos
        public int Cantidad { get; set; } // La cantidad del producto en el carrito

        // Propiedades de navegación
        // Estas son útiles si estás utilizando un ORM que soporte la carga de entidades relacionadas
        // Por ejemplo, si quieres cargar los detalles del producto o del usuario asociado a este carrito directamente
        public virtual Usuario Usuario { get; set; } // Representa la relación con la tabla Usuarios
        public virtual Producto Producto { get; set; } // Representa la relación con la tabla Productos
    }

}