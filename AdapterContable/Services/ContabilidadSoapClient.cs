using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using AdapterContable.Models;

namespace AdapterContable.Services
{
    public class ContabilidadSoapClient
    {
        private readonly string _serviceUrl;

        public ContabilidadSoapClient(string serviceUrl)
        {
            _serviceUrl = serviceUrl;
            Console.WriteLine(string.Format("Cliente SOAP configurado: {0}", serviceUrl));
        }

        public EstadoCuenta RegistrarPago(string clienteId, decimal monto)
        {
            try
            {
                string soapEnvelope = CrearSoapEnvelope(clienteId, monto);

                string soapResponse = InvocarServicio(soapEnvelope);

                EstadoCuenta estado = ParsearRespuesta(soapResponse, clienteId, monto);

                return estado;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error al invocar SOAP: {0}", ex.Message));
                throw;
            }
        }

        private string CrearSoapEnvelope(string clienteId, decimal monto)
        {
            return string.Format(
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">
  <soap:Body>
    <tem:RegistrarPago>
      <tem:clienteId>{0}</tem:clienteId>
      <tem:monto>{1}</tem:monto>
    </tem:RegistrarPago>
  </soap:Body>
</soap:Envelope>",
                clienteId,
                monto.ToString(System.Globalization.CultureInfo.InvariantCulture)
            );
        }

        private string InvocarServicio(string soapEnvelope)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_serviceUrl);
            request.Method = "POST";
            request.ContentType = "text/xml; charset=utf-8";
            request.Headers.Add("SOAPAction", "http://tempuri.org/IContabilidadService/RegistrarPago");

            byte[] bytes = Encoding.UTF8.GetBytes(soapEnvelope);
            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        private EstadoCuenta ParsearRespuesta(string soapResponse, string clienteId, decimal montoPagado)
        {
            try
            {
                XDocument doc = XDocument.Parse(soapResponse);

                XNamespace soapNs = "http://schemas.xmlsoap.org/soap/envelope/";
                XNamespace tempNs = "http://tempuri.org/";
                XNamespace dataNs = "http://schemas.datacontract.org/2004/07/SistemaContable";

                var body = doc.Root.Element(soapNs + "Body");
                if (body == null)
                {
                    throw new Exception("No se encontro el elemento Body en la respuesta SOAP");
                }

                var registrarPagoResponse = body.Element(tempNs + "RegistrarPagoResponse");
                if (registrarPagoResponse == null)
                {
                    throw new Exception("No se encontro el elemento RegistrarPagoResponse en la respuesta SOAP");
                }

                var registrarPagoResult = registrarPagoResponse.Element(tempNs + "RegistrarPagoResult");
                if (registrarPagoResult == null)
                {
                    registrarPagoResult = registrarPagoResponse.Element("RegistrarPagoResult");
                }
                if (registrarPagoResult == null)
                {
                    throw new Exception("No se encontro el elemento RegistrarPagoResult en la respuesta SOAP");
                }

                var clienteIdElement = registrarPagoResult.Element(dataNs + "ClienteId");
                if (clienteIdElement == null)
                {
                    throw new Exception("No se encontro el elemento ClienteId en RegistrarPagoResult");
                }

                var saldoElement = registrarPagoResult.Element(dataNs + "Saldo");
                if (saldoElement == null)
                {
                    throw new Exception("No se encontro el elemento Saldo en RegistrarPagoResult");
                }

                string clienteIdResponse = clienteIdElement.Value;
                decimal saldo = decimal.Parse(saldoElement.Value, System.Globalization.CultureInfo.InvariantCulture);

                EstadoCuenta estado = new EstadoCuenta();
                estado.clienteId = clienteIdResponse;
                estado.saldo = saldo;
                estado.montoPagado = montoPagado;
                estado.fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                estado.estado = saldo <= 0 ? "HABILITADO" : "DESHABILITADO";

                return estado;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error al parsear respuesta SOAP: {0}", ex.Message));
                throw;
            }
        }
    }
}
