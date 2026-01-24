using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Script.Serialization;
using AdapterWeb.Models;

namespace AdapterWeb.Services
{
    /// <summary>
    /// Cliente para consumir API REST de WebPagos
    /// Patrón: Channel Adapter - conecta API REST con sistema de mensajería
    /// </summary>
    public class WebPagosClient
    {
        private readonly string _baseUrl;
        private readonly JavaScriptSerializer _jsonSerializer;

        public WebPagosClient(string baseUrl)
        {
            _baseUrl = baseUrl;
            _jsonSerializer = new JavaScriptSerializer();
        }

        /// <summary>
        /// Obtiene los pagos del día desde el endpoint /api/pagos/today
        /// </summary>
        /// <returns>Lista de pagos del día</returns>
        public List<PagoWeb> ObtenerPagosDelDia()
        {
            string url = string.Format("{0}/api/pagos/today", _baseUrl);

            Console.WriteLine(string.Format("GET {0}", url));

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";

                    string jsonResponse = client.DownloadString(url);

                    Console.WriteLine(string.Format("Respuesta recibida: {0} bytes", jsonResponse.Length));

                    List<PagoWeb> pagos = _jsonSerializer.Deserialize<List<PagoWeb>>(jsonResponse);

                    Console.WriteLine(string.Format("Pagos deserializados: {0}", pagos.Count));

                    return pagos;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(string.Format("Error HTTP: {0}", ex.Message));
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error al obtener pagos: {0}", ex.Message));
                throw;
            }
        }
    }
}
