package cl.insi.aukan.adapter.controlacceso;

import cl.insi.aukan.adapter.controlacceso.client.ControlAccesoRestClient;
import cl.insi.aukan.adapter.controlacceso.handler.EstadoMessageHandler;
import cl.insi.aukan.adapter.controlacceso.jms.JmsEstadoConsumer;

public class ControlAccesoAdapterApplication {

    private static final String ACTIVEMQ_URL = "tcp://localhost:61616";
    private static final String TOPIC_NAME = "smi_estados";
    private static final String SUBSCRIPTION_NAME = "smi_estados.controlacceso";
    private static final String CONTROL_ACCESO_URL = "http://localhost:5003";

    public static void main(String[] args) {
        try {
            ControlAccesoRestClient restClient = new ControlAccesoRestClient(CONTROL_ACCESO_URL);
            EstadoMessageHandler handler = new EstadoMessageHandler(restClient);
            JmsEstadoConsumer consumer = new JmsEstadoConsumer(
                    ACTIVEMQ_URL,
                    TOPIC_NAME,
                    SUBSCRIPTION_NAME,
                    handler
            );
            consumer.start();

        } catch (Exception e) {
            System.err.println("Error fatal: " + e.getMessage());
            e.printStackTrace();
            System.exit(1);
        }
    }
}
