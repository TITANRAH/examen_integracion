package cl.insi.aukan.adapter.agendamiento;

import cl.insi.aukan.adapter.agendamiento.handler.EstadoMessageHandler;
import cl.insi.aukan.adapter.agendamiento.jms.JmsEstadoConsumer;
import cl.insi.aukan.adapter.agendamiento.soap.AgendamientoSoapClient;

/**
 * Adapter Agendamiento
 *
 * Patrón EIP: Channel Adapter
 *
 * Responsabilidad:
 * - Consumir estados de cuenta desde ActiveMQ (tópico smi_estados)
 * - Determinar si cliente debe ser habilitado/deshabilitado en agendamiento
 * - Invocar servicio SOAP del Sistema de Agendamiento
 *
 * Lógica:
 * - saldo <= 0 → HabilitarUsuario (SOAP)
 * - saldo > 0  → DeshabilitarUsuario (SOAP)
 */
public class AgendamientoAdapterApplication {

    private static final String ACTIVEMQ_URL = "tcp://localhost:61616";
    private static final String TOPIC_NAME = "smi_estados";
    private static final String SUBSCRIPTION_NAME = "smi_estados.agendamiento";
    private static final String AGENDAMIENTO_SOAP_URL = "http://localhost:5002/AgendaService.svc";

    public static void main(String[] args) {
        try {
            // Crear componentes
            AgendamientoSoapClient soapClient = new AgendamientoSoapClient(AGENDAMIENTO_SOAP_URL);
            EstadoMessageHandler handler = new EstadoMessageHandler(soapClient);
            JmsEstadoConsumer consumer = new JmsEstadoConsumer(
                    ACTIVEMQ_URL,
                    TOPIC_NAME,
                    SUBSCRIPTION_NAME,
                    handler
            );

            // Iniciar consumidor
            consumer.start();

        } catch (Exception e) {
            System.err.println("Error fatal: " + e.getMessage());
            e.printStackTrace();
            System.exit(1);
        }
    }
}
