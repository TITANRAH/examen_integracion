using System;
using System.IO;
using System.Net;
using System.Text;

namespace WebPagosMock
{
    /// <summary>
    /// Servidor REST Mock para WebPagos
    /// Simula el endpoint GET /api/pagos/today
    /// </summary>
    class Program
    {
        private const int DEFAULT_PORT = 5000;

        static void Main(string[] args)
        {
            int port = DEFAULT_PORT;
            if (args.Length > 0)
            {
                int.TryParse(args[0], out port);
            }

            string url = string.Format("http://localhost:{0}/", port);

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(url);

            try
            {
                listener.Start();
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine("  WebPagos Mock Server");
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine(string.Format("Escuchando en: {0}", url));
                Console.WriteLine("Endpoint: GET /api/pagos/today");
                Console.WriteLine();
                Console.WriteLine("Presiona Ctrl+C para detener...");
                Console.WriteLine("════════════════════════════════════════");
                Console.WriteLine();

                while (true)
                {
                    HttpListenerContext context = listener.GetContext();
                    ProcessRequest(context);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0}", ex.Message));
            }
            finally
            {
                listener.Stop();
            }
        }

        static void ProcessRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            Console.WriteLine(string.Format("{0} {1}", request.HttpMethod, request.Url.PathAndQuery));

            string responseString = "";
            int statusCode = 200;

            if (request.HttpMethod == "GET" && request.Url.PathAndQuery == "/api/pagos/today")
            {
                // JSON con pagos de ejemplo
                responseString = @"[
  {
    ""rut"": ""15111222-2"",
    ""monto"": 15000,
    ""formaPago"": ""TC"",
    ""codigoAutorizacion"": ""AUTH001"",
    ""tarjeta"": ""VISA"",
    ""fecha"": ""2026-01-24T10:30:00""
  },
  {
    ""rut"": ""16111222-2"",
    ""monto"": 10000,
    ""formaPago"": ""EF"",
    ""fecha"": ""2026-01-24T11:15:00""
  },
  {
    ""rut"": ""17111222-2"",
    ""monto"": 20000,
    ""formaPago"": ""TD"",
    ""codigoAutorizacion"": ""AUTH002"",
    ""tarjeta"": ""MASTERCARD"",
    ""fecha"": ""2026-01-24T12:00:00""
  }
]";
                Console.WriteLine(string.Format("  -> Respondiendo con {0} pagos", 3));
            }
            else
            {
                statusCode = 404;
                responseString = string.Format("{{\"error\": \"Endpoint no encontrado: {0}\"}}", request.Url.PathAndQuery);
                Console.WriteLine("  -> 404 Not Found");
            }

            response.StatusCode = statusCode;
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}
