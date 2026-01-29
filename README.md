# Sistema de Integración Aukan Gym

## Comandos de Ejecución

### Web Pagos (REST)

```bash
.\WebPagos.exe <puerto>
```

- Puerto por defecto: 5000
- Endpoints: `GET /api/payments`, `GET /api/payments/today`

### Sistema Contable (SOAP)

```bash
.\SistemaContable.exe <puerto>
```

- Puerto por defecto: 5001
- WSDL: `http://localhost:5001/?wsdl`
- Requisito: .NET Framework 4.8.1 (Solo Windows)

### Sistema de Agendamiento (SOAP)

```bash
dotnet SistemaAgendamientoClases.dll <puerto>
```

- Puerto por defecto: 5002
- WSDL: `http://localhost:5002/AgendaService?wsdl`
- Requisito: .NET 8

### Sistema de Control de Acceso (REST)

```bash
dotnet SistemaControlAcceso.dll
```

- Requisito: .NET 10
- Endpoints: `GET /api/users`, `POST /api/users`, `GET /api/users/{rut}`, `PATCH /api/users/{rut}`, `DELETE /api/users/{rut}`
