package cl.insi.aukan.bridge;

import javax.jms.*;
import org.apache.activemq.artemis.jms.client.ActiveMQConnectionFactory;

/**
 * Bridge MSMQ → ActiveMQ
 *
 * Patrón EIP: Messaging Bridge
 *
 * Responsabilidad:
 * - Recibir mensaje desde MSMQ Trigger (como argumento)
 * - Publicar mensaje en ActiveMQ SIN modificarlo
 * - Transferir de Windows (MSMQ) a Linux (ActiveMQ)
 *
 * Uso:
 *   java -jar bridge.jar "<mensaje_json>"
 */
public class BridgeApplication {

    private static final String ACTIVEMQ_URL = "tcp://localhost:61616";
    private static final String TOPIC_NAME = "smi_estados";
    private static final String USERNAME = "admin";
    private static final String PASSWORD = "admin";

    public static void main(String[] args) {
        // Validar argumentos
        if (args.length < 1) {
            System.err.println("ERROR: Falta el mensaje a transferir");
            System.err.println("Uso: java -jar bridge.jar \"<mensaje>\"");
            System.exit(1);
        }

        String messageBody = args[0];

        System.out.println("═══════════════════════════════════════");
        System.out.println("  Bridge MSMQ → ActiveMQ");
        System.out.println("═══════════════════════════════════════");
        System.out.println("ActiveMQ URL: " + ACTIVEMQ_URL);
        System.out.println("Tópico destino: " + TOPIC_NAME);
        System.out.println("Mensaje a transferir: " + messageBody.substring(0, Math.min(100, messageBody.length())) + "...");
        System.out.println();

        try {
            publishToActiveMQ(messageBody);
            System.out.println("✅ Mensaje transferido exitosamente a ActiveMQ");
            System.exit(0);
        } catch (Exception e) {
            System.err.println("❌ Error al transferir mensaje: " + e.getMessage());
            e.printStackTrace();
            System.exit(1);
        }
    }

    private static void publishToActiveMQ(String messageBody) throws JMSException {
        // Crear conexión a ActiveMQ
        try (ActiveMQConnectionFactory factory = new ActiveMQConnectionFactory(ACTIVEMQ_URL, USERNAME, PASSWORD);
             Connection connection = factory.createConnection();
             Session session = connection.createSession(false, Session.AUTO_ACKNOWLEDGE)) {

            connection.start();

            // Crear tópico y publicador
            Topic topic = session.createTopic(TOPIC_NAME);
            MessageProducer producer = session.createProducer(topic);
            producer.setDeliveryMode(DeliveryMode.PERSISTENT);

            // Crear mensaje de texto (NO modificar el contenido)
            TextMessage message = session.createTextMessage(messageBody);

            // Publicar mensaje
            producer.send(message);

            System.out.println("→ Mensaje publicado en tópico: " + TOPIC_NAME);
        }
    }
}
