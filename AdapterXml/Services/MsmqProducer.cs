using System;
using System.IO;
using System.Messaging;
using System.Text;
using System.Xml.Serialization;
using AdapterXml.Models;

namespace AdapterXml.Services
{
    public class MsmqProducer : IDisposable
    {
        private readonly MessageQueue _queue;
        private readonly string _queuePath;

        public MsmqProducer(string queuePath)
        {
            _queuePath = queuePath;

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

        public void PublicarPago(Pago pago, string sucursalId, string fechaOriginal)
        {
            try
            {
                string xmlContent = SerializarPagoAXml(pago);

                var message = new Message
                {
                    Body = xmlContent,
                    Label = string.Format("Pago {0} - Sucursal {1}", pago.Rut, sucursalId),
                    Recoverable = true
                };

                message.Extension = Encoding.UTF8.GetBytes(string.Format("SUCURSAL:{0}|FECHA:{1}", sucursalId, fechaOriginal));
                _queue.Send(message);

                Console.WriteLine(string.Format("  ✅ Publicado: {0} → {1}", pago, _queuePath));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("  ❌ Error al publicar pago: {0}", ex.Message));
                throw;
            }
        }

        private string SerializarPagoAXml(Pago pago)
        {
            var serializer = new XmlSerializer(typeof(Pago));
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, pago);
                return stringWriter.ToString();
            }
        }

        public int ObtenerCantidadMensajes()
        {
            try
            {
                var messages = _queue.GetAllMessages();
                return messages.Length;
            }
            catch
            {
                return -1;
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
