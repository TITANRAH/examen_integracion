# Script para crear colas MSMQ - Examen INSI Aukan Gym
# Alumno: Sergio Miranda
# Prefijo: smi
# Fecha: 2026-01-24

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Creación de Colas MSMQ - Aukan Gym" -ForegroundColor Cyan
Write-Host "Alumno: Sergio Miranda" -ForegroundColor Cyan
Write-Host "Prefijo: smi" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Cargar ensamblado de MSMQ
Add-Type -AssemblyName System.Messaging

# Definir colas a crear con prefijo smi
$queues = @(
    @{
        Path = ".\Private$\smi_suc_pagos"
        Description = "Cola para pagos XML de sucursales"
    },
    @{
        Path = ".\Private$\smi_web_pagos"
        Description = "Cola para pagos JSON de WebPagos"
    },
    @{
        Path = ".\Private$\smi_pagos"
        Description = "Cola para pagos canónicos (Datatype Channel)"
    },
    @{
        Path = ".\Private$\smi_estados"
        Description = "Cola para estados de cuenta hacia Bridge"
    }
)

# Crear cada cola
foreach ($queue in $queues) {
    $queuePath = $queue.Path
    $description = $queue.Description

    Write-Host "Verificando: $queuePath" -ForegroundColor Yellow

    if ([System.Messaging.MessageQueue]::Exists($queuePath)) {
        Write-Host "  ⚠️  Cola ya existe: $queuePath" -ForegroundColor Yellow
    } else {
        try {
            # Crear cola privada
            $newQueue = [System.Messaging.MessageQueue]::Create($queuePath)

            # Configurar propiedades
            $newQueue.Label = $description

            Write-Host "  ✅ Cola creada exitosamente: $queuePath" -ForegroundColor Green
            Write-Host "     Descripción: $description" -ForegroundColor Gray
        } catch {
            Write-Host "  ❌ Error al crear cola: $queuePath" -ForegroundColor Red
            Write-Host "     Error: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    Write-Host ""
}

# Verificar y listar todas las colas creadas
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Verificación de Colas Creadas" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Obtener solo las colas con prefijo smi
$smiQueues = Get-MsmqQueue -QueueType Private | Where-Object { $_.QueueName -like "*smi_*" }

if ($smiQueues) {
    Write-Host "Colas MSMQ encontradas con prefijo 'smi':" -ForegroundColor Green
    Write-Host ""

    $smiQueues | Format-Table -Property @(
        @{Label="Nombre"; Expression={$_.QueueName}; Width=20},
        @{Label="Ruta Completa"; Expression={$_.Name}; Width=40},
        @{Label="Mensajes"; Expression={$_.MessageCount}; Width=10}
    ) -AutoSize

    Write-Host ""
    Write-Host "Total de colas creadas: $($smiQueues.Count)" -ForegroundColor Green

    # Verificar que tenemos las 4 colas esperadas
    if ($smiQueues.Count -eq 4) {
        Write-Host "✅ ¡Todas las colas requeridas han sido creadas exitosamente!" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Se esperaban 4 colas, pero se encontraron $($smiQueues.Count)" -ForegroundColor Yellow
    }
} else {
    Write-Host "❌ No se encontraron colas con prefijo 'smi'" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Script completado" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
