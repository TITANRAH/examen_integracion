using System;
using TranslatorJson.Models;

namespace TranslatorJson.Services
{
    public class JsonToCanonicalTransformer
    {
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
