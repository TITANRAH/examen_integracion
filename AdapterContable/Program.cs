using System;
using AdapterContable.Services;

namespace AdapterContable
{
    /// <summary>
    /// Adapter Contable - Actividad 6
    /// Patrón EIP: Channel Adapter
    ///
    /// Responsabilidad:
    /// - Consumir pagos canónicos de cola smi_pagos
    /// - Invocar servicio SOAP RegistrarPago(clienteId, monto)
    /// - Recibir estado de cuenta del cliente
    /// - Publicar estado en cola smi_estados
    ///
    /// Alumno: Sergio Miranda
    /// Prefijo: smi
    /// </summary>
    class Program
    {
        private const string QUEUE_ORIGEN = @".\Private$\smi_pagos";
        private const string QUEUE_DESTINO = @".\Private$\smi_estados";
        private const string DEFAULT_SOAP_URL = "http://localhost:5001/ContabilidadService";

        static void Main(string[] args)
        {
            MostrarBanner();

            try
            {
                string soapUrl = args.Length > 0 ? args[0] : DEFAULT_SOAP_URL;

                Console.WriteLine(string.Format("URL del servicio SOAP: {0}", soapUrl));
                Console.WriteLine();

                ProcesarPagos(soapUrl);

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

        private static void ProcesarPagos(string soapUrl)
        {
            var soapClient = new ContabilidadSoapClient(soapUrl);
            int totalProcesados = 0;
            int totalHabilitados = 0;
            int totalDeshabilitados = 0;

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

                Console.WriteLine("Procesando pagos...");
                Console.WriteLine("────────────────────────────────────────");

                while (true)
                {
                    var pagoCanonical = consumer.LeerPagoCanonical();

                    if (pagoCanonical == null)
                    {
                        break;
                    }

                    Console.WriteLine(string.Format("Leído: {0}", pagoCanonical));

                    // Invocar servicio SOAP
                    Console.WriteLine(string.Format("  Invocando SOAP: RegistrarPago({0}, {1})", pagoCanonical.rut, pagoCanonical.monto));

                    var estadoCuenta = soapClient.RegistrarPago(pagoCanonical.rut, pagoCanonical.monto);

                    Console.WriteLine(string.Format("  Respuesta SOAP: {0}", estadoCuenta));

                    // Publicar estado en MSMQ
                    producer.PublicarEstadoCuenta(estadoCuenta);

                    totalProcesados++;

                    if (estadoCuenta.EstaHabilitado())
                    {
                        totalHabilitados++;
                    }
                    else
                    {
                        totalDeshabilitados++;
                    }

                    Console.WriteLine();
                }

                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine("Resumen de Procesamiento");
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine(string.Format("Pagos procesados: {0}", totalProcesados));
                Console.WriteLine(string.Format("Clientes habilitados: {0}", totalHabilitados));
                Console.WriteLine(string.Format("Clientes deshabilitados: {0}", totalDeshabilitados));

                int mensajesEnDestino = producer.ObtenerCantidadMensajes();
                if (mensajesEnDestino >= 0)
                {
                    Console.WriteLine(string.Format("Estados en {0}: {1}", QUEUE_DESTINO, mensajesEnDestino));
                }
            }
        }

        private static void MostrarBanner()
        {
            Console.Clear();
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine("  Adapter Contable - Aukan Gym");
            Console.WriteLine("  Actividad 6: Channel Adapter (SOAP)");
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
