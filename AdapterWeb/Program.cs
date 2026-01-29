using System;
using AdapterWeb.Services;

namespace AdapterWeb
{
    class Program
    {
        private const string QUEUE_PATH = @".\Private$\smi_web_pagos";
        private const string DEFAULT_API_URL = "http://localhost:5000";

        static void Main(string[] args)
        {
            MostrarBanner();

            try
            {
                string apiUrl = args.Length > 0 ? args[0] : DEFAULT_API_URL;

                Console.WriteLine(string.Format("URL del API WebPagos: {0}", apiUrl));
                Console.WriteLine();

                ProcesarPagos(apiUrl);

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

        private static void ProcesarPagos(string apiUrl)
        {
            var webClient = new WebPagosClient(apiUrl);
            int totalPagos = 0;

            using (var msmqProducer = new MsmqProducer(QUEUE_PATH))
            {
                Console.WriteLine("────────────────────────────────────────");
                Console.WriteLine("Obteniendo pagos del día desde API REST...");
                Console.WriteLine();

                var pagos = webClient.ObtenerPagosDelDia();

                if (pagos.Count == 0)
                {
                    Console.WriteLine("No se encontraron pagos del día");
                    return;
                }

                Console.WriteLine();
                Console.WriteLine(string.Format("Publicando {0} pago(s) en MSMQ...", pagos.Count));

                foreach (var pago in pagos)
                {
                    msmqProducer.PublicarPago(pago);
                    totalPagos++;
                }

                Console.WriteLine();
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine("Resumen de Procesamiento");
                Console.WriteLine("════════════════════════════════════════");
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
            Console.WriteLine("  Adapter Web - Aukan Gym");
            Console.WriteLine("  Actividad 3: Channel Adapter");
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine("  Alumno: Sergio Miranda");
            Console.WriteLine("  Prefijo: smi");
            Console.WriteLine(string.Format("  Cola destino: {0}", QUEUE_PATH));
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine();
        }
    }
}
