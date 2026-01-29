package cl.insi.aukan.adapter.agendamiento.handler;

import cl.insi.aukan.adapter.agendamiento.model.EstadoCuenta;
import cl.insi.aukan.adapter.agendamiento.soap.AgendamientoSoapClient;
import com.google.gson.Gson;

public class EstadoMessageHandler {
    private final AgendamientoSoapClient soapClient;
    private final Gson gson;

    public EstadoMessageHandler(AgendamientoSoapClient soapClient) {
        this.soapClient = soapClient;
        this.gson = new Gson();
    }

    public void handleMessage(String jsonMessage) {
        try {
            EstadoCuenta estado = gson.fromJson(jsonMessage, EstadoCuenta.class);

            String rut = estado.getEstadoCuenta().getCliente().getRut();
            int saldo = estado.getEstadoCuenta().getCuenta().getSaldo();
            String estadoStr = estado.getEstadoCuenta().getCuenta().getEstado();
            boolean habilitar = saldo <= 0;

            System.out.println("  Estado recibido:");
            System.out.println("    RUT: " + rut);
            System.out.println("    Saldo: $" + saldo);
            System.out.println("    Estado: " + estadoStr);
            System.out.println("    Acción SOAP: " + (habilitar ? "HabilitarUsuario" : "DeshabilitarUsuario"));

            if (habilitar) {
                soapClient.habilitarUsuario(rut);
            } else {
                soapClient.deshabilitarUsuario(rut);
            }

        } catch (Exception e) {
            System.err.println("  ✗ Error al procesar mensaje: " + e.getMessage());
            e.printStackTrace();
        }
    }
}
