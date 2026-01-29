package cl.insi.aukan.adapter.controlacceso.handler;

import cl.insi.aukan.adapter.controlacceso.client.ControlAccesoRestClient;
import cl.insi.aukan.adapter.controlacceso.model.EstadoCuenta;
import com.google.gson.Gson;

public class EstadoMessageHandler {
    private final ControlAccesoRestClient restClient;
    private final Gson gson;

    public EstadoMessageHandler(ControlAccesoRestClient restClient) {
        this.restClient = restClient;
        this.gson = new Gson();
    }

    public void handleMessage(String jsonMessage) {
        try {
            EstadoCuenta estado = gson.fromJson(jsonMessage, EstadoCuenta.class);

            String rut = estado.getEstadoCuenta().getCliente().getRut();
            int saldo = estado.getEstadoCuenta().getCuenta().getSaldo();
            String estadoStr = estado.getEstadoCuenta().getCuenta().getEstado();
            boolean habilitado = saldo <= 0;

            System.out.println("  Estado recibido:");
            System.out.println("    RUT: " + rut);
            System.out.println("    Saldo: $" + saldo);
            System.out.println("    Estado: " + estadoStr);
            System.out.println("    Acción: " + (habilitado ? "HABILITAR" : "DESHABILITAR"));

            restClient.actualizarUsuario(rut, habilitado);

        } catch (Exception e) {
            System.err.println("  ✗ Error al procesar mensaje: " + e.getMessage());
            e.printStackTrace();
        }
    }
}
