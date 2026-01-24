using System;
using System.IO;
using System.Linq;
using AdapterXml.Services;

namespace AdapterXml
{
    /// <summary>
    /// Adapter XML - Actividad 2
    /// Patrón EIP: Channel Adapter
    ///
    /// Responsabilidad:
    /// - Leer archivos XML de sucursales (suc_XXX-pagos-fecha.xml)
    /// - Extraer cada pago individual
    /// - Publicar cada pago en cola MSMQ smi_suc_pagos SIN transformar
    ///
    /// Alumno: Sergio Miranda
    /// Prefijo: smi
    /// </summary>
    class Program
    {
        // Configuración
        private const string QUEUE_PATH = @".\Private$\smi_suc_pagos";
        private const string DEFAULT_XML_DIRECTORY = @"..\..\..\..\XMLPagos";

        static void Main(string[] args)
        {
            MostrarBanner();

            try
            {
                // Determinar directorio de archivos XML
                string xmlDirectory = args.Length > 0 ? args[0] : DEFAULT_XML_DIRECTORY;
                xmlDirectory = Path.GetFullPath(xmlDirectory);

                if (!Directory.Exists(xmlDirectory))
                {
                    Console.WriteLine(string.Format("❌ Directorio no encontrado: {0}", xmlDirectory));
                    Console.WriteLine("💡 Uso: AdapterXml.exe [ruta-directorio-xml]");
                    Environment.Exit(1);
                }

                Console.WriteLine(string.Format("📁 Directorio de archivos XML: {0}", xmlDirectory));
                Console.WriteLine();

                // Buscar archivos XML de sucursales
                var archivosXml = Directory.GetFiles(xmlDirectory, "suc_*-pagos-*.xml")
                                          .OrderBy(f => f)
                                          .ToList();

                if (archivosXml.Count == 0)
                {
                    Console.WriteLine("⚠️  No se encontraron archivos XML con patrón 'suc_*-pagos-*.xml'");
                    Environment.Exit(0);
                }

                Console.WriteLine(string.Format("📋 Archivos encontrados: {0}", archivosXml.Count));
                foreach (var archivo in archivosXml)
                {
                    Console.WriteLine(string.Format("   - {0}", Path.GetFileName(archivo)));
                }
                Console.WriteLine();

                // Procesar archivos
                ProcesarArchivos(archivosXml);

                Console.WriteLine();
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine("✅ Proceso completado exitosamente");
                Console.WriteLine("════════════════════════════════════════");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine(string.Format("❌ Error fatal: {0}", ex.Message));
                Console.WriteLine("════════════════════════════════════════");
                Environment.Exit(1);
            }

            // Mantener consola abierta en modo debug
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine();
                Console.WriteLine("Presione cualquier tecla para salir...");
                Console.ReadKey();
            }
        }

        private static void ProcesarArchivos(System.Collections.Generic.List<string> archivos)
        {
            var xmlReader = new XmlFileReader();
            int totalPagos = 0;
            int totalArchivos = 0;

            using (var msmqProducer = new MsmqProducer(QUEUE_PATH))
            {
                foreach (var archivo in archivos)
                {
                    try
                    {
                        Console.WriteLine("────────────────────────────────────────");

                        // Leer archivo XML
                        var pagos = xmlReader.LeerArchivoPagos(archivo);
                        var sucursalId = xmlReader.ExtraerIdSucursal(Path.GetFileName(archivo));

                        if (pagos.ListaPagos.Count == 0)
                        {
                            Console.WriteLine("⚠️  Archivo sin pagos, omitiendo...");
                            continue;
                        }

                        // Publicar cada pago individualmente
                        Console.WriteLine(string.Format("📤 Publicando {0} pago(s) en MSMQ...", pagos.ListaPagos.Count));

                        foreach (var pago in pagos.ListaPagos)
                        {
                            msmqProducer.PublicarPago(pago, sucursalId, pagos.Fecha);
                            totalPagos++;
                        }

                        totalArchivos++;
                        Console.WriteLine("✅ Archivo procesado completamente");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("❌ Error procesando archivo {0}: {1}", Path.GetFileName(archivo), ex.Message));
                        // Continuar con el siguiente archivo
                    }
                }

                Console.WriteLine();
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine("📊 Resumen de Procesamiento");
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine(string.Format("Archivos procesados: {0}/{1}", totalArchivos, archivos.Count));
                Console.WriteLine(string.Format("Pagos publicados: {0}", totalPagos));

                int mensajesEnCola = msmqProducer.ObtenerCantidadMensajes();
                if (mensajesEnCola >= 0)
                {
                    Console.WriteLine(string.Format("Mensajes en cola {0}: {1}", QUEUE_PATH, mensajesEnCola));
                }
            }
        }

        private static void MostrarBanner()
        {
            Console.Clear();
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine("  Adapter XML - Aukan Gym");
            Console.WriteLine("  Actividad 2: Channel Adapter");
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine("  Alumno: Sergio Miranda");
            Console.WriteLine("  Prefijo: smi");
            Console.WriteLine(string.Format("  Cola destino: {0}", QUEUE_PATH));
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine();
        }
    }
}
