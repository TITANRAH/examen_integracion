using System;
using TranslatorXml.Models;

namespace TranslatorXml.Services
{
    public class XmlToCanonicalTransformer
    {
        public PagoCanonical Transform(Pago pagoXml, string metadatos)
        {
            string sucursalId = ExtraerSucursalId(metadatos);
            string fecha = ExtraerFecha(metadatos);

            PagoCanonical pagoCanonical = new PagoCanonical
            {
                origen = "sucursal",
                sucursalId = sucursalId,
                rut = pagoXml.Rut,
                monto = pagoXml.Monto,
                formaPago = pagoXml.FormaPago,
                fecha = ConvertirFechaAIso8601(fecha),
                codigoAutorizacion = pagoXml.CodigoAutorizacion,
                tarjeta = pagoXml.Tarjeta,
                timestampProcesamiento = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            };

            return pagoCanonical;
        }

        private string ExtraerSucursalId(string metadatos)
        {
            if (string.IsNullOrEmpty(metadatos))
            {
                return "UNKNOWN";
            }

            string[] partes = metadatos.Split('|');
            foreach (string parte in partes)
            {
                if (parte.StartsWith("SUCURSAL:"))
                {
                    return parte.Substring(9);
                }
            }

            return "UNKNOWN";
        }

        private string ExtraerFecha(string metadatos)
        {
            if (string.IsNullOrEmpty(metadatos))
            {
                return DateTime.Now.ToString("yyyy-MM-dd");
            }

            string[] partes = metadatos.Split('|');
            foreach (string parte in partes)
            {
                if (parte.StartsWith("FECHA:"))
                {
                    return parte.Substring(6);
                }
            }

            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        private string ConvertirFechaAIso8601(string fecha)
        {
            try
            {
                DateTime dt = DateTime.Parse(fecha);
                return dt.ToString("yyyy-MM-ddTHH:mm:ss");
            }
            catch
            {
                return DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            }
        }
    }
}
