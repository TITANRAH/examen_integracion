package cl.insi.aukan.adapter.agendamiento.soap;

import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.nio.charset.StandardCharsets;

public class AgendamientoSoapClient {
    private final String soapUrl;

    public AgendamientoSoapClient(String soapUrl) {
        this.soapUrl = soapUrl;
    }

    public void habilitarUsuario(String clienteId) throws Exception {
        String soapRequest = buildSoapRequest("HabilitarUsuario", clienteId);
        invokeSoap(soapRequest);
        System.out.println("    ✓ Usuario habilitado en agendamiento: " + clienteId);
    }

    public void deshabilitarUsuario(String clienteId) throws Exception {
        String soapRequest = buildSoapRequest("DeshabilitarUsuario", clienteId);
        invokeSoap(soapRequest);
        System.out.println("    ✓ Usuario deshabilitado en agendamiento: " + clienteId);
    }

    private String buildSoapRequest(String methodName, String clienteId) {
        return "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" " +
                "xmlns:tem=\"http://tempuri.org/\">" +
                "<soap:Body>" +
                "<tem:" + methodName + ">" +
                "<tem:clienteId>" + clienteId + "</tem:clienteId>" +
                "</tem:" + methodName + ">" +
                "</soap:Body>" +
                "</soap:Envelope>";
    }

    private void invokeSoap(String soapRequest) throws Exception {
        URL url = new URL(soapUrl);
        HttpURLConnection connection = (HttpURLConnection) url.openConnection();

        connection.setRequestMethod("POST");
        connection.setRequestProperty("Content-Type", "text/xml; charset=utf-8");
        connection.setRequestProperty("SOAPAction", "");
        connection.setDoOutput(true);

        try (OutputStream os = connection.getOutputStream()) {
            byte[] input = soapRequest.getBytes(StandardCharsets.UTF_8);
            os.write(input, 0, input.length);
        }

        int responseCode = connection.getResponseCode();
        if (responseCode < 200 || responseCode >= 300) {
            throw new Exception("Error SOAP: HTTP " + responseCode);
        }
    }
}
