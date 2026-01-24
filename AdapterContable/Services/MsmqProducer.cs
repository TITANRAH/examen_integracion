using System;
using System.Messaging;
using System.Web.Script.Serialization;
using AdapterContable.Models;

namespace AdapterContable.Services
{
    /// <summary>
    /// Productor de mensajes MSMQ
    /// Publica estados de cuenta JSON en la cola destino
    /// </summary>
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

            Console.WriteLine(string.Format("Conectado a cola destino: {0}", queuePath));
        }

        /// <summary>
        /// Publica un estado de cuenta en MSMQ como JSON
        /// </summary>
        public void PublicarEstadoCuenta(EstadoCuenta estado)
        {
            try
            {
                string jsonContent = _jsonSerializer.Serialize(estado);

                var message = new Message
                {
                    Body = jsonContent,
                    Label = string.Format("Estado {0}", estado.clienteId),
                    Recoverable = true
                };

                _queue.Send(message);

                Console.WriteLine(string.Format("  Publicado: {0}", estado));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("  Error al publicar: {0}", ex.Message));
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
