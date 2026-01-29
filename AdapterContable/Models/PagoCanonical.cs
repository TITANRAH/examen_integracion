using System;

namespace AdapterContable.Models
{
    public class PagoCanonical
    {
        public string origen { get; set; }
        public string sucursalId { get; set; }
        public string rut { get; set; }
        public decimal monto { get; set; }
        public string formaPago { get; set; }
        public string fecha { get; set; }
        public string codigoAutorizacion { get; set; }
        public string tarjeta { get; set; }
        public string timestampProcesamiento { get; set; }

        public override string ToString()
        {
            return string.Format("Pago: {0} [{1}] - ${2}", rut, origen, monto);
        }
    }
}
