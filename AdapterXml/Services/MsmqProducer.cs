using System;
using System.IO;
using System.Messaging;
using System.Text;
using System.Xml.Serialization;
using AdapterXml.Models;

namespace AdapterXml.Services
{
    /// <summary>
    /// Productor de mensajes MSMQ.
    /// Publica pagos XML individuales en la cola smi_suc_pagos
    /// </summary>
    public class MsmqProducer : IDisposable
    {
        private readonly MessageQueue _queue;
        private readonly string _queuePath;

        public MsmqProducer(string queuePath)
        {
            _queuePath = queuePath;

            // Verificar que la cola exists
            if (!MessageQueue.Exists(queuePath))
            {
                throw new InvalidOperationException(string.Format("La cola MSMQ no existe: {0}", queuePath));
            }

            _queue = new MessageQueue(queuePath)
            {
                Formatter = new XmlMessageFormatter(new Type[] { typeof(string) })
            };

            Console.WriteLine(string.Format("🔗 Conectado a cola MSMQ: {0}", queuePath));
        }

        /// <summary>
        /// Publica un pago individual en MSMQ como XML
        /// </summary>
        /// <param name="pago">Pago a publicar</param>
        /// <param name="sucursalId">ID de la sucursal de origen</param>
        /// <param name="fechaOriginal">Fecha original del archivo</param>
        public void PublicarPago(Pago pago, string sucursalId, string fechaOriginal)
        {
            try
            {
                // Serializar el pago a XML
                string xmlContent = SerializarPagoAXml(pago);

                // Crear mensaje MSMQ
                var message = new Message
                {
                    Body = xmlContent,
                    Label = string.Format("Pago {0} - Sucursal {1}", pago.Rut, sucursalId),
                    Recoverable = true // Mensaje persistente
                };

                // Agregar propiedades personalizadas para trazabilidad
                message.Extension = Encoding.UTF8.GetBytes(string.Format("SUCURSAL:{0}|FECHA:{1}", sucursalId, fechaOriginal));

                // Enviar a MSMQ
                _queue.Send(message);

                Console.WriteLine(string.Format("  ✅ Publicado: {0} → {1}", pago, _queuePath));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("  ❌ Error al publicar pago: {0}", ex.Message));
                throw;
            }
        }

        /// <summary>
        /// Serializa un objeto Pago a string XML
        /// </summary>
        private string SerializarPagoAXml(Pago pago)
        {
            var serializer = new XmlSerializer(typeof(Pago));
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, pago);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// Obtiene el número de mensajes actualmente en la cola
        /// </summary>
        public int ObtenerCantidadMensajes()
        {
            try
            {
                var messages = _queue.GetAllMessages();
                return messages.Length;
            }
            catch
            {
                return -1; // Error al obtener cantidad
            }
        }

        public void Dispose()
        {
            if (_queue != null)
            {
                _queue.Dispose();
            }
        }
    }
}
