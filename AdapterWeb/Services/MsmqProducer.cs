using System;
using System.Messaging;
using System.Text;
using System.Web.Script.Serialization;
using AdapterWeb.Models;

namespace AdapterWeb.Services
{
    public class MsmqProducer : IDisposable
    {
        private readonly MessageQueue _queue;
        private readonly string _queuePath;
        private readonly JavaScriptSerializer _jsonSerializer;

        public MsmqProducer(string queuePath)
        {
            _queuePath = queuePath;
            _jsonSerializer = new JavaScriptSerializer();

            if (!MessageQueue.Exists(queuePath))
            {
                throw new InvalidOperationException(string.Format("La cola MSMQ no existe: {0}", queuePath));
            }

            _queue = new MessageQueue(queuePath)
            {
                Formatter = new XmlMessageFormatter(new Type[] { typeof(string) })
            };

            Console.WriteLine(string.Format("Conectado a cola MSMQ: {0}", queuePath));
        }

        public void PublicarPago(PagoWeb pago)
        {
            try
            {
                string jsonContent = _jsonSerializer.Serialize(pago);

                var message = new Message
                {
                    Body = jsonContent,
                    Label = string.Format("Pago Web {0}", pago.rut),
                    Recoverable = true
                };

                _queue.Send(message);

                Console.WriteLine(string.Format("  Publicado: {0}", pago));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("  Error al publicar pago: {0}", ex.Message));
                throw;
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
