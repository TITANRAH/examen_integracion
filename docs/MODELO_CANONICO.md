# Modelo de Mensaje Canónico
## Sistema de Integración Aukan Gym

**Alumno:** Sergio Miranda
**Actividad 4:** Diseño del Modelo Canónico
**Fecha:** 24 Enero 2026

---

## 1. Objetivo

Definir un formato JSON **unificado y estandarizado** que actúe como "lenguaje común" entre:
- **Fuentes:** XMLPagos (sucursales) y WebPagos (online)
- **Destinos:** Sistema Contable, Control de Acceso, Agendamiento

Este modelo viaja por el canal central `smi_pagos` y es consumido por todos los sistemas downstream.

---

## 2. Análisis de Fuentes

### 2.1 XMLPagos (Sucursales)

**Formato:** XML
**Estructura:**
```xml
<Pagos fecha="2026-01-20">
  <Pago>
    <Rut>15111222-2</Rut>
    <Monto>15000</Monto>
    <FormaPago>TC</FormaPago>
    <CodigoAutorizacion>AUTH001</CodigoAutorizacion>
    <Tarjeta>VISA</Tarjeta>
  </Pago>
</Pagos>
```

**Metadatos adicionales:**
- `sucursalId`: Extraído del nombre de archivo (ej: `suc_001-pagos-20260120.xml` → `001`)

### 2.2 WebPagos (Online)

**Formato:** JSON
**Estructura:**
```json
{
  "rut": "15111222-2",
  "monto": 15000,
  "formaPago": "TC",
  "codigoAutorizacion": "AUTH001",
  "tarjeta": "VISA",
  "fecha": "2026-01-24T10:30:00"
}
```

---

## 3. Decisiones de Diseño

### 3.1 Convenciones de Nombres

✅ **Adoptar camelCase** (estándar JSON)
- Razón: Es el estándar de facto en APIs REST modernas
- Ejemplo: `formaPago` (no `FormaPago`)

### 3.2 Campos Obligatorios

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `origen` | string | Fuente del pago: `"sucursal"` o `"web"` |
| `rut` | string | RUT del cliente (formato: 12345678-9) |
| `monto` | number | Monto del pago en CLP |
| `formaPago` | string | Forma de pago: `"TC"`, `"TD"`, `"EF"` |
| `fecha` | string | Fecha/hora del pago (ISO 8601) |

### 3.3 Campos Opcionales

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `sucursalId` | string | ID de sucursal (solo para origen=sucursal) |
| `codigoAutorizacion` | string | Código de autorización (TC/TD) |
| `tarjeta` | string | Tipo de tarjeta: `"VISA"`, `"MASTERCARD"`, etc. |

### 3.4 Campos de Trazabilidad

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `timestampProcesamiento` | string | Timestamp cuando se procesó el mensaje (ISO 8601) |

---

## 4. Estructura del Modelo Canónico

### 4.1 Schema JSON

```json
{
  "origen": "string (sucursal|web)",
  "rut": "string",
  "monto": "number",
  "formaPago": "string (TC|TD|EF)",
  "fecha": "string (ISO 8601)",
  "sucursalId": "string (opcional, solo si origen=sucursal)",
  "codigoAutorizacion": "string (opcional)",
  "tarjeta": "string (opcional)",
  "timestampProcesamiento": "string (ISO 8601)"
}
```

### 4.2 Ejemplo 1: Pago de Sucursal

**Entrada (XML):**
```xml
<Pago>
  <Rut>15111222-2</Rut>
  <Monto>15000</Monto>
  <FormaPago>TC</FormaPago>
  <CodigoAutorizacion>AUTH001</CodigoAutorizacion>
  <Tarjeta>VISA</Tarjeta>
</Pago>
```
Archivo: `suc_001-pagos-20260120.xml`

**Salida (Modelo Canónico):**
```json
{
  "origen": "sucursal",
  "sucursalId": "001",
  "rut": "15111222-2",
  "monto": 15000,
  "formaPago": "TC",
  "fecha": "2026-01-20T00:00:00",
  "codigoAutorizacion": "AUTH001",
  "tarjeta": "VISA",
  "timestampProcesamiento": "2026-01-24T14:25:00"
}
```

### 4.3 Ejemplo 2: Pago Web

**Entrada (JSON):**
```json
{
  "rut": "16111222-2",
  "monto": 10000,
  "formaPago": "EF",
  "fecha": "2026-01-24T11:15:00"
}
```

**Salida (Modelo Canónico):**
```json
{
  "origen": "web",
  "rut": "16111222-2",
  "monto": 10000,
  "formaPago": "EF",
  "fecha": "2026-01-24T11:15:00",
  "timestampProcesamiento": "2026-01-24T14:25:00"
}
```

---

## 5. Justificación de Decisiones

### 5.1 ¿Por qué incluir campo `origen`?

- Permite **trazabilidad** del origen del pago
- Útil para **auditoría** y **análisis** de fuentes
- Facilita **debugging** si hay problemas con una fuente específica

### 5.2 ¿Por qué `sucursalId` es opcional?

- Solo aplica para pagos de origen `"sucursal"`
- Pagos web no tienen sucursal asociada
- Mantiene el modelo flexible y sin campos nulos innecesarios

### 5.3 ¿Por qué agregar `timestampProcesamiento`?

- **Trazabilidad:** Saber cuándo fue procesado el mensaje
- **Orden:** Útil para ordenar mensajes por fecha de procesamiento
- **Debugging:** Identificar retrasos en el pipeline

### 5.4 ¿Por qué usar ISO 8601 para fechas?

- Estándar internacional
- Incluye timezone
- Compatible con todas las plataformas (Java, C#, JavaScript)
- Ejemplo: `2026-01-24T14:25:00`

---

## 6. Validaciones

### 6.1 Validaciones de Negocio

| Campo | Validación |
|-------|------------|
| `rut` | Formato válido chileno (Ej: 12345678-9) |
| `monto` | > 0 |
| `formaPago` | Enum: TC, TD, EF |
| `origen` | Enum: sucursal, web |

### 6.2 Reglas de Consistencia

- Si `formaPago` = "TC" o "TD" → Debe tener `codigoAutorizacion`
- Si `formaPago` = "TC" o "TD" → Debe tener `tarjeta`
- Si `origen` = "sucursal" → Debe tener `sucursalId`
- Si `formaPago` = "EF" → `codigoAutorizacion` y `tarjeta` deben ser null/omitidos

---

## 7. Extensibilidad

El modelo está diseñado para ser extensible:

**Futuras fuentes:**
- Agregar nuevos valores a `origen` (ej: `"app-movil"`, `"kiosko"`)
- Agregar campos opcionales sin romper compatibilidad

**Futuros campos:**
- `metodoPago.detalles` (objeto con detalles específicos)
- `cliente.nombre` (información adicional del cliente)
- `idTransaccion` (ID único generado)

---

## 8. Beneficios del Modelo Canónico

✅ **Desacoplamiento:** Fuentes y destinos no se conocen entre sí
✅ **Estandarización:** Un solo formato para todos los sistemas
✅ **Mantenibilidad:** Cambios en una fuente no afectan a destinos
✅ **Trazabilidad:** Información de origen y procesamiento
✅ **Escalabilidad:** Fácil agregar nuevas fuentes o destinos

---

## 9. Uso en el Pipeline

```
┌─────────────┐
│  AdapterXML │ ──(XML)──> [TranslatorXML] ──┐
└─────────────┘                              │
                                             ├──(JSON Canónico)──> [smi_pagos]
┌─────────────┐                              │
│ AdapterWeb  │ ──(JSON)──> [TranslatorJSON]─┘
└─────────────┘

[smi_pagos] ──(JSON Canónico)──> AdapterContable
                              ──> AdapterControlAcceso
                              ──> AdapterAgendamiento
```

**Responsabilidad de los Translators:**
- Transformar formatos específicos (XML/JSON) al Modelo Canónico
- Agregar campos de trazabilidad (`origen`, `timestampProcesamiento`)
- Validar datos según reglas de negocio

---

**Fin del Diseño del Modelo Canónico**
