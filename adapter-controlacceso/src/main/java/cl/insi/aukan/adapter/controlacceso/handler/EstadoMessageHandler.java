package cl.insi.aukan.adapter.controlacceso.handler;

import cl.insi.aukan.adapter.controlacceso.client.ControlAccesoRestClient;
import cl.insi.aukan.adapter.controlacceso.model.EstadoCuenta;
import com.google.gson.Gson;

/**
 * Handler para procesar mensajes de estado de cuenta
 *
 * Lógica de negocio:
 * - saldo <= 0 → Cliente al día → HABILITAR acceso
 * - saldo > 0  → Cliente con deuda → DESHABILITAR acceso
 */
public class EstadoMessageHandler {
    private final ControlAccesoRestClient restClient;
    private final Gson gson;

    public EstadoMessageHandler(ControlAccesoRestClient restClient) {
        this.restClient = restClient;
        this.gson = new Gson();
    }

    public void handleMessage(String jsonMessage) {
        try {
            // Deserializar JSON → EstadoCuenta
            EstadoCuenta estado = gson.fromJson(jsonMessage, EstadoCuenta.class);

            String rut = estado.getEstadoCuenta().getCliente().getRut();
            int saldo = estado.getEstadoCuenta().getCuenta().getSaldo();
            String estadoStr = estado.getEstadoCuenta().getCuenta().getEstado();

            // Lógica de habilitación
            boolean habilitado = saldo <= 0;

            System.out.println("  Estado recibido:");
            System.out.println("    RUT: " + rut);
            System.out.println("    Saldo: $" + saldo);
            System.out.println("    Estado: " + estadoStr);
            System.out.println("    Acción: " + (habilitado ? "HABILITAR" : "DESHABILITAR"));

            // Invocar API REST
            restClient.actualizarUsuario(rut, habilitado);

        } catch (Exception e) {
            System.err.println("  ✗ Error al procesar mensaje: " + e.getMessage());
            e.printStackTrace();
        }
    }
}
