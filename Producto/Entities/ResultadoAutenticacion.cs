using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Producto.Entities
{
    public class ResultadoAutenticacion
    {
        public bool IsAuthenticated { get; set; }
        public string Role { get; set; }
    }
}