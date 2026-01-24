package cl.insi.aukan.adapter.controlacceso.jms;

import cl.insi.aukan.adapter.controlacceso.handler.EstadoMessageHandler;
import org.apache.activemq.artemis.jms.client.ActiveMQConnectionFactory;

import javax.jms.*;

/**
 * Consumidor JMS para tópico smi_estados
 *
 * Patrón EIP: Publish-Subscribe (Topic)
 * Suscripción durable: smi_estados.controlacceso
 */
public class JmsEstadoConsumer {
    private final String activeMqUrl;
    private final String topicName;
    private final String subscriptionName;
    private final EstadoMessageHandler handler;

    public JmsEstadoConsumer(String activeMqUrl, String topicName, String subscriptionName,
                             EstadoMessageHandler handler) {
        this.activeMqUrl = activeMqUrl;
        this.topicName = topicName;
        this.subscriptionName = subscriptionName;
        this.handler = handler;
    }

    public void start() throws JMSException {
        System.out.println("═══════════════════════════════════════");
        System.out.println("  Adapter Control de Acceso");
        System.out.println("═══════════════════════════════════════");
        System.out.println("ActiveMQ URL: " + activeMqUrl);
        System.out.println("Tópico: " + topicName);
        System.out.println("Suscripción: " + subscriptionName);
        System.out.println();
        System.out.println("Escuchando mensajes de estado de cuenta...");
        System.out.println();

        ActiveMQConnectionFactory factory = new ActiveMQConnectionFactory(activeMqUrl, "admin", "admin");
        Connection connection = factory.createConnection();
        connection.setClientID("adapter-controlacceso");
        connection.start();

        Session session = connection.createSession(false, Session.AUTO_ACKNOWLEDGE);
        Topic topic = session.createTopic(topicName);

        // Suscripción durable
        MessageConsumer consumer = session.createDurableSubscriber(topic, subscriptionName);

        // Listener asíncrono
        consumer.setMessageListener(message -> {
            if (message instanceof TextMessage) {
                try {
                    String body = ((TextMessage) message).getText();
                    System.out.println("─────────────────────────────────────");
                    handler.handleMessage(body);
                } catch (JMSException e) {
                    System.err.println("Error al leer mensaje: " + e.getMessage());
                }
            }
        });

        System.out.println("✓ Consumidor iniciado. Presione Ctrl+C para detener.");

        // Mantener la aplicación corriendo
        Object lock = new Object();
        synchronized (lock) {
            try {
                lock.wait();
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
            }
        }
    }
}
