using System;
using TranslatorXml.Models;

namespace TranslatorXml.Services
{
    /// <summary>
    /// Transformer XML a Modelo Canónico
    /// Patrón EIP: Message Translator
    /// Transforma mensajes XML de sucursales al formato JSON canónico
    /// </summary>
    public class XmlToCanonicalTransformer
    {
        /// <summary>
        /// Transforma un Pago XML + metadatos a PagoCanonical JSON
        /// </summary>
        /// <param name="pagoXml">Pago deserializado del XML</param>
        /// <param name="metadatos">Metadatos del mensaje (SUCURSAL:XXX|FECHA:YYYY-MM-DD)</param>
        /// <returns>Pago en formato canónico</returns>
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

        /// <summary>
        /// Extrae el ID de sucursal de los metadatos
        /// Formato esperado: SUCURSAL:001|FECHA:2026-01-20
        /// </summary>
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

        /// <summary>
        /// Extrae la fecha de los metadatos
        /// Formato esperado: SUCURSAL:001|FECHA:2026-01-20
        /// </summary>
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

        /// <summary>
        /// Convierte fecha YYYY-MM-DD a formato ISO 8601
        /// </summary>
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
