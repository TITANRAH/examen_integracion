using System;

namespace AdapterWeb.Models
{
    public class PagoWeb
    {
        public string rut { get; set; }
        public decimal monto { get; set; }
        public string formaPago { get; set; }
        public string codigoAutorizacion { get; set; }
        public string tarjeta { get; set; }
        public string fecha { get; set; }

        public override string ToString()
        {
            return string.Format("Pago: {0} - ${1} ({2})", rut, monto, formaPago);
        }
    }
}
