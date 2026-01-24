using System;
using TranslatorJson.Models;

namespace TranslatorJson.Services
{
    /// <summary>
    /// Transformer JSON a Modelo Canónico
    /// Patrón EIP: Message Translator
    /// Transforma mensajes JSON de WebPagos al formato JSON canónico
    /// </summary>
    public class JsonToCanonicalTransformer
    {
        /// <summary>
        /// Transforma un PagoWeb JSON a PagoCanonical JSON
        /// </summary>
        /// <param name="pagoWeb">Pago deserializado del JSON de WebPagos</param>
        /// <returns>Pago en formato canónico</returns>
        public PagoCanonical Transform(PagoWeb pagoWeb)
        {
            PagoCanonical pagoCanonical = new PagoCanonical
            {
                origen = "web",
                sucursalId = null,
                rut = pagoWeb.rut,
                monto = pagoWeb.monto,
                formaPago = pagoWeb.formaPago,
                fecha = pagoWeb.fecha,
                codigoAutorizacion = pagoWeb.codigoAutorizacion,
                tarjeta = pagoWeb.tarjeta,
                timestampProcesamiento = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            };

            return pagoCanonical;
        }
    }
}
