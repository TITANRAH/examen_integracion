using System;
using System.IO;
using System.Xml.Serialization;
using AdapterXml.Models;

namespace AdapterXml.Services
{
    public class XmlFileReader
    {
        private readonly XmlSerializer _serializer;

        public XmlFileReader()
        {
            _serializer = new XmlSerializer(typeof(Pagos));
        }

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

        public string ExtraerIdSucursal(string fileName)
        {
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
