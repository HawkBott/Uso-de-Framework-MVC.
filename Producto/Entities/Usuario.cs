using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Producto.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string MyEmail { get; set; }
        public string MyPassword { get; set; }

        // Propiedad para el ID del Rol
        public int RolId { get; set; }

        // Propiedad de navegación para el Rol
        // Esto es útil en ORM como Entity Framework para cargar los detalles del Rol
        public Rol Rol { get; set; }


        public int CarritoId { get; set; }
    }
}