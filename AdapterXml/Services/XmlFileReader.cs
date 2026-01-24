using System;
using System.IO;
using System.Xml.Serialization;
using AdapterXml.Models;

namespace AdapterXml.Services
{
    /// <summary>
    /// Servicio para leer archivos XML de pagos de sucursales.
    /// Patrón: Channel Adapter - conecta archivos XML con sistema de mensajería
    /// </summary>
    public class XmlFileReader
    {
        private readonly XmlSerializer _serializer;

        public XmlFileReader()
        {
            _serializer = new XmlSerializer(typeof(Pagos));
        }

        /// <summary>
        /// Lee y deserializa un archivo XML de pagos
        /// </summary>
        /// <param name="filePath">Ruta completa al archivo XML</param>
        /// <returns>Objeto Pagos deserializado</returns>
        /// <exception cref="FileNotFoundException">Si el archivo no existe</exception>
        /// <exception cref="InvalidOperationException">Si el XML es inválido</exception>
        public Pagos LeerArchivoPagos(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Format("Archivo no encontrado: {0}", filePath));
            }

            Console.WriteLine(string.Format("📄 Leyendo archivo: {0}", Path.GetFileName(filePath)));

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    var pagos = (Pagos)_serializer.Deserialize(reader);

                    if (pagos == null)
                    {
                        throw new InvalidOperationException("No se pudo deserializar el archivo XML");
                    }

                    Console.WriteLine(string.Format("✅ Archivo procesado: {0} pago(s) encontrado(s)", pagos.ListaPagos.Count));
                    Console.WriteLine(string.Format("   Fecha: {0}", pagos.Fecha));

                    return pagos;
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(string.Format("❌ Error al parsear XML: {0}", ex.Message));
                throw;
            }
        }

        /// <summary>
        /// Extrae el identificador de sucursal del nombre de archivo
        /// Ejemplo: "suc_001-pagos-20260120.xml" → "001"
        /// </summary>
        /// <param name="fileName">Nombre del archivo</param>
        /// <returns>Identificador de sucursal (ej: "001", "002", "003")</returns>
        public string ExtraerIdSucursal(string fileName)
        {
            // Formato esperado: suc_XXX-pagos-fecha.xml
            if (fileName.StartsWith("suc_") && fileName.Contains("-"))
            {
                var partes = fileName.Split('_', '-');
                if (partes.Length >= 2)
                {
                    return partes[1]; // Retorna "001", "002", etc.
                }
            }

            return "UNKNOWN";
        }
    }
}
