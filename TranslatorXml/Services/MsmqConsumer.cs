using System;
using System.IO;
using System.Messaging;
using System.Xml.Serialization;
using TranslatorXml.Models;

namespace TranslatorXml.Services
{
    public class MsmqConsumer : IDisposable
    {
        private readonly MessageQueue _queue;
        private readonly string _queuePath;
        private readonly XmlSerializer _serializer;

        public MsmqConsumer(string queuePath)
        {
            _queuePath = queuePath;
            _serializer = new XmlSerializer(typeof(Pago));

            if (!MessageQueue.Exists(queuePath))
            {
                throw new InvalidOperationException(string.Format("La cola MSMQ no existe: {0}", queuePath));
            }

            _queue = new MessageQueue(queuePath)
            {
                Formatter = new XmlMessageFormatter(new Type[] { typeof(string) })
            };

            _queue.MessageReadPropertyFilter.Extension = true;

            Console.WriteLine(string.Format("Conectado a cola origen: {0}", queuePath));
        }

        public Pago LeerPago(out string metadatos)
        {
            metadatos = "";

            try
            {
                Message message = _queue.Receive(TimeSpan.FromSeconds(1));

                string xmlContent = message.Body.ToString();

                metadatos = System.Text.Encoding.UTF8.GetString(message.Extension);

                using (StringReader reader = new StringReader(xmlContent))
                {
                    Pago pago = (Pago)_serializer.Deserialize(reader);
                    return pago;
                }
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
