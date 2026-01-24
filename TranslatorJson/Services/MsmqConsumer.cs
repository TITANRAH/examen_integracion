using System;
using System.Messaging;
using System.Web.Script.Serialization;
using TranslatorJson.Models;

namespace TranslatorJson.Services
{
    /// <summary>
    /// Consumidor de mensajes MSMQ
    /// Lee mensajes JSON de la cola de origen
    /// </summary>
    public class MsmqConsumer : IDisposable
    {
        private readonly MessageQueue _queue;
        private readonly string _queuePath;
        private readonly JavaScriptSerializer _jsonSerializer;

        public MsmqConsumer(string queuePath)
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

            Console.WriteLine(string.Format("Conectado a cola origen: {0}", queuePath));
        }

        /// <summary>
        /// Lee y deserializa un mensaje JSON de MSMQ
        /// Retorna null si no hay mensajes
        /// </summary>
        public PagoWeb LeerPago()
        {
            try
            {
                Message message = _queue.Receive(TimeSpan.FromSeconds(1));

                string jsonContent = message.Body.ToString();

                PagoWeb pago = _jsonSerializer.Deserialize<PagoWeb>(jsonContent);

                return pago;
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                {
                    return null;
                }
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
