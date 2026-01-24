package cl.insi.aukan.adapter.controlacceso.client;

import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;

/**
 * Cliente REST para Sistema de Control de Acceso
 *
 * API:
 * - PATCH /api/users/{rut}
 * - Body: {"habilitado": true/false}
 */
public class ControlAccesoRestClient {
    private final String baseUrl;
    private final HttpClient httpClient;

    public ControlAccesoRestClient(String baseUrl) {
        this.baseUrl = baseUrl;
        this.httpClient = HttpClient.newHttpClient();
    }

    /**
     * Actualizar estado de habilitación de un usuario
     *
     * @param rut       RUT del cliente (formato: 12345678-9)
     * @param habilitado true para habilitar, false para deshabilitar
     */
    public void actualizarUsuario(String rut, boolean habilitado) throws Exception {
        String url = baseUrl + "/api/users/" + rut;
        String jsonBody = "{\"habilitado\": " + habilitado + "}";

        HttpRequest request = HttpRequest.newBuilder()
                .uri(URI.create(url))
                .header("Content-Type", "application/json")
                .method("PATCH", HttpRequest.BodyPublishers.ofString(jsonBody))
                .build();

        HttpResponse<String> response = httpClient.send(request, HttpResponse.BodyHandlers.ofString());

        if (response.statusCode() >= 200 && response.statusCode() < 300) {
            System.out.println("    ✓ Usuario actualizado: " + rut + " → " +
                    (habilitado ? "HABILITADO" : "DESHABILITADO"));
        } else {
            System.err.println("    ✗ Error al actualizar usuario: HTTP " + response.statusCode());
            System.err.println("      Respuesta: " + response.body());
        }
    }
}
