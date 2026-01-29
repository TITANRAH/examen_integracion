using System;

namespace AdapterContable.Models
{
    public class EstadoCuenta
    {
        public string clienteId { get; set; }
        public decimal saldo { get; set; }
        public decimal montoPagado { get; set; }
        public string fecha { get; set; }
        public string estado { get; set; }

        public EstadoCuenta()
        {
            clienteId = "";
            fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            estado = "";
        }

        public bool EstaHabilitado()
        {
            return saldo <= 0;
        }

        public override string ToString()
        {
            string estadoStr = EstaHabilitado() ? "HABILITADO" : "DESHABILITADO";
            return string.Format("Estado: {0} - Saldo: ${1} - {2}", clienteId, saldo, estadoStr);
        }
    }
}
