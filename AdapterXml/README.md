# Adapter XML - Actividad 2

**Patrón EIP:** Channel Adapter
**Alumno:** Sergio Miranda
**Prefijo:** `smi`

## Descripción

Componente que conecta archivos XML de sucursales con el sistema de mensajería MSMQ. Implementa el patrón **Channel Adapter** para integrar una fuente de datos externa (archivos) con la arquitectura de mensajería.

## Responsabilidades

1. ✅ Leer archivos XML de sucursales del directorio `XMLPagos/`
2. ✅ Parsear estructura XML usando `System.Xml.Serialization`
3. ✅ Extraer cada elemento `<Pago>` individualmente
4. ✅ Publicar cada pago como mensaje XML en cola MSMQ `smi_suc_pagos`
5. ✅ **NO transforma** el formato (responsabilidad del Translator en Actividad 5)

## Estructura del Proyecto

```
AdapterXml/
├── AdapterXml.csproj       # Proyecto .NET Framework 4.8.1
├── App.config              # Configuración
├── Program.cs              # Punto de entrada
├── Models/
│   ├── Pago.cs            # Modelo XML para <Pago>
│   └── Pagos.cs           # Modelo XML para <Pagos>
└── Services/
    ├── XmlFileReader.cs   # Lectura y deserialización XML
    └── MsmqProducer.cs    # Publicación en MSMQ
```

## Tecnologías

- **.NET Framework 4.8.1**
- **System.Messaging** (MSMQ)
- **System.Xml.Serialization** (JAXB equivalente)

## Compilación

### En Windows con .NET SDK instalado:

```powershell
# Navegar al directorio del proyecto
cd AdapterXml

# Compilar
dotnet build

# O con MSBuild
msbuild AdapterXml.csproj
```

## Ejecución

### Opción 1: Sin parámetros (usa directorio por defecto)

```powershell
.\bin\Debug\net481\AdapterXml.exe
```

Busca archivos en: `..\..\..\..\XMLPagos`

### Opción 2: Con directorio personalizado

```powershell
.\bin\Debug\net481\AdapterXml.exe "C:\ruta\a\archivos\xml"
```

## Requisitos Previos

1. ✅ MSMQ instalado y servicio corriendo
2. ✅ Cola `.\Private$\smi_suc_pagos` creada
3. ✅ Archivos XML con formato: `suc_XXX-pagos-YYYYMMDD.xml`

## Formato XML de Entrada

```xml
<Pagos fecha="2026-01-20">
    <Pago>
        <Rut>5111222-2</Rut>
        <Monto>15000</Monto>
        <FormaPago>TC</FormaPago>
        <CodigoAutorizacion>564654</CodigoAutorizacion>
        <Tarjeta>0020</Tarjeta>
    </Pago>
    <Pago>
        <Rut>16111222-2</Rut>
        <Monto>10000</Monto>
        <FormaPago>EF</FormaPago>
    </Pago>
</Pagos>
```

## Salida Esperada

Cada `<Pago>` se publica como mensaje individual en MSMQ `smi_suc_pagos` conservando el formato XML original.

**Ejemplo de log:**

```
════════════════════════════════════════
  Adapter XML - Aukan Gym
  Actividad 2: Channel Adapter
════════════════════════════════════════
  Alumno: Sergio Miranda
  Prefijo: smi
  Cola destino: .\Private$\smi_suc_pagos
════════════════════════════════════════

📁 Directorio de archivos XML: C:\...\XMLPagos

📋 Archivos encontrados: 3
   - suc_001-pagos-20260120.xml
   - suc_002-pagos-20260120.xml
   - suc_003-pagos-20260120.xml

────────────────────────────────────────
📄 Leyendo archivo: suc_001-pagos-20260120.xml
✅ Archivo procesado: 2 pago(s) encontrado(s)
   Fecha: 2026-01-20
📤 Publicando 2 pago(s) en MSMQ...
  ✅ Publicado: Pago: 5111222-2 - $15000 (TC) → .\Private$\smi_suc_pagos
  ✅ Publicado: Pago: 16111222-2 - $10000 (EF) → .\Private$\smi_suc_pagos
✅ Archivo procesado completamente

════════════════════════════════════════
📊 Resumen de Procesamiento
════════════════════════════════════════
Archivos procesados: 3/3
Pagos publicados: 5
Mensajes en cola .\Private$\smi_suc_pagos: 5

════════════════════════════════════════
✅ Proceso completado exitosamente
════════════════════════════════════════
```

## Verificación

Verificar mensajes en cola MSMQ:

```powershell
Get-MsmqQueue -Name "smi_suc_pagos"
```

## Patrones EIP Implementados

- **Channel Adapter:** Conecta fuente externa (archivos XML) con messaging system (MSMQ)
- **Point-to-Point Channel:** Cola MSMQ garantiza procesamiento por un solo consumidor
- **Message:** Cada pago XML es un mensaje independiente

## Próximo Componente

**Translator XML (Actividad 5):** Consumirá mensajes de `smi_suc_pagos` y transformará al formato canónico JSON.
