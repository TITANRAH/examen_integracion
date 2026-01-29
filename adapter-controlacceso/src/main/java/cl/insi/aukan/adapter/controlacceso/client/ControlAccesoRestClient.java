package cl.insi.aukan.adapter.controlacceso.client;

import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;

public class ControlAccesoRestClient {
    private final String baseUrl;
    private final HttpClient httpClient;

    public ControlAccesoRestClient(String baseUrl) {
        this.baseUrl = baseUrl;
        this.httpClient = HttpClient.newHttpClient();
    }

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
