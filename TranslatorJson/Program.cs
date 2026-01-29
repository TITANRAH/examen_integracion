using System;
using TranslatorJson.Services;

namespace TranslatorJson
{
    class Program
    {
        private const string QUEUE_ORIGEN = @".\Private$\smi_web_pagos";
        private const string QUEUE_DESTINO = @".\Private$\smi_pagos";

        static void Main(string[] args)
        {
            MostrarBanner();

            try
            {
                ProcesarMensajes();

                Console.WriteLine();
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine("Proceso completado exitosamente");
                Console.WriteLine("════════════════════════════════════════");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine(string.Format("Error fatal: {0}", ex.Message));
                Console.WriteLine("════════════════════════════════════════");
                Environment.Exit(1);
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine();
                Console.WriteLine("Presione cualquier tecla para salir...");
                Console.ReadKey();
            }
        }

        private static void ProcesarMensajes()
        {
            var transformer = new JsonToCanonicalTransformer();
            int totalProcesados = 0;

            using (var consumer = new MsmqConsumer(QUEUE_ORIGEN))
            using (var producer = new MsmqProducer(QUEUE_DESTINO))
            {
                int mensajesDisponibles = consumer.ObtenerCantidadMensajes();
                Console.WriteLine(string.Format("Mensajes disponibles en {0}: {1}", QUEUE_ORIGEN, mensajesDisponibles));
                Console.WriteLine();

                if (mensajesDisponibles == 0)
                {
                    Console.WriteLine("No hay mensajes para procesar");
                    return;
                }

                Console.WriteLine("Procesando mensajes...");
                Console.WriteLine("────────────────────────────────────────");

                while (true)
                {
                    var pagoWeb = consumer.LeerPago();

                    if (pagoWeb == null)
                    {
                        break;
                    }

                    Console.WriteLine(string.Format("Leído: {0}", pagoWeb));

                    var pagoCanonical = transformer.Transform(pagoWeb);

                    producer.PublicarPagoCanonical(pagoCanonical);

                    totalProcesados++;
                }

                Console.WriteLine();
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine("Resumen de Procesamiento");
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine(string.Format("Mensajes procesados: {0}", totalProcesados));

                int mensajesEnDestino = producer.ObtenerCantidadMensajes();
                if (mensajesEnDestino >= 0)
                {
                    Console.WriteLine(string.Format("Mensajes en {0}: {1}", QUEUE_DESTINO, mensajesEnDestino));
                }
            }
        }

        private static void MostrarBanner()
        {
            Console.Clear();
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine("  Translator JSON - Aukan Gym");
            Console.WriteLine("  Actividad 5: Message Translator");
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine("  Alumno: Sergio Miranda");
            Console.WriteLine("  Prefijo: smi");
            Console.WriteLine(string.Format("  Cola origen: {0}", QUEUE_ORIGEN));
            Console.WriteLine(string.Format("  Cola destino: {0}", QUEUE_DESTINO));
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine();
        }
    }
}
