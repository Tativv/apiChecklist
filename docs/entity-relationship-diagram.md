# Diagrama entidad-relación — Dominio de Checklists

Generado a partir del modelo de dominio tras el rediseño del sistema de recurrencia
(commit `ConvertToGenericSchedule`). Refleja el estado real de `HotelChecklist.Domain.Entities`
y las migraciones de PostgreSQL aplicadas.

## Idea central

Un `Schedule` es una regla de ejecución genérica (frecuencia + horario) que **no se comparte**:
cada fila de `TemplateSchedule` o `TaskSchedule` es dueña de su propio `Schedule`.

- `TemplateSchedule` decide **qué días** se genera una `ChecklistInstance` para un template.
- `TaskSchedule` decide **cuántas veces y a qué hora** se ejecuta una tarea dentro de esa
  instancia ya generada (una tarea `Scheduled` con 3 `TaskSchedule` produce 3
  `ChecklistTaskExecution` el mismo día; una tarea `Continuous` produce 1 sin horario).

## Diagrama

```mermaid
erDiagram
    AREA ||--o{ ASSET : contiene
    AREA ||--o{ CHECKLIST_TEMPLATE : agrupa

    CHECKLIST_TEMPLATE ||--o{ CHECKLIST_TASK : define
    CHECKLIST_TEMPLATE ||--o{ TEMPLATE_SCHEDULE : "se ejecuta según"
    CHECKLIST_TEMPLATE ||--o{ TEMPLATE_ASSET : "se aplica a"
    CHECKLIST_TEMPLATE ||--o{ CHECKLIST_INSTANCE : genera

    SCHEDULE ||--|| TEMPLATE_SCHEDULE : "regla de"
    SCHEDULE ||--|| TASK_SCHEDULE : "regla de"
    SCHEDULE |o--o{ CHECKLIST_TASK_EXECUTION : originó

    CHECKLIST_TASK ||--o{ TASK_SCHEDULE : "se ejecuta según"
    CHECKLIST_TASK ||--o{ CHECKLIST_TASK_EXECUTION : instancia

    ASSET ||--o{ TEMPLATE_ASSET : "asociado en"
    ASSET ||--o{ CHECKLIST_INSTANCE : "es objeto de"

    CHECKLIST_INSTANCE ||--o{ CHECKLIST_TASK_EXECUTION : contiene
    CHECKLIST_INSTANCE }o--o| USER : "asignado a / aprobado por"

    CHECKLIST_TASK_EXECUTION ||--o{ CHECKLIST_TASK_EVIDENCE : adjunta
    CHECKLIST_TASK_EXECUTION }o--o| USER : "completado por"

    CHECKLIST_TASK_EVIDENCE }o--|| USER : "subido por"

    AREA {
        uuid id PK
        string name
    }

    ASSET {
        uuid id PK
        string name
        string type
        uuid area_id FK
        bool active
    }

    USER {
        uuid id PK
        string name
        string email
        string role
        bool active
    }

    CHECKLIST_TEMPLATE {
        uuid id PK
        string name
        string description
        uuid area_id FK
        int estimated_duration_minutes
    }

    CHECKLIST_TASK {
        uuid id PK
        uuid template_id FK
        string name
        string description
        int order
        enum execution_mode "Scheduled | Continuous"
    }

    SCHEDULE {
        uuid id PK
        enum frequency_type "Daily | Weekly | Monthly"
        int interval_value "cada N unidades"
        enum week_day "nullable, Weekly"
        int day_of_month "nullable, Monthly, 1-31"
        time time_of_day
        int execution_order
        bool active
        timestamptz created_at_utc "ancla de intervalo"
    }

    TEMPLATE_SCHEDULE {
        uuid id PK
        uuid template_id FK
        uuid schedule_id FK
    }

    TASK_SCHEDULE {
        uuid id PK
        uuid task_id FK
        uuid schedule_id FK
    }

    TEMPLATE_ASSET {
        uuid id PK
        uuid template_id FK
        uuid asset_id FK
        timestamptz created_at_utc
    }

    CHECKLIST_INSTANCE {
        uuid id PK
        uuid template_id FK
        uuid asset_id FK
        date date
        enum status "Pending | InProgress | Completed | Approved"
        timestamptz started_at
        timestamptz completed_at
        long duration_seconds
        uuid assigned_user_id FK
        uuid approved_by_user_id FK
        timestamptz approved_at
    }

    CHECKLIST_TASK_EXECUTION {
        uuid id PK
        uuid checklist_instance_id FK
        uuid task_id FK
        uuid schedule_id FK "nullable, SET NULL"
        timestamptz scheduled_for_utc "nullable"
        timestamptz executed_at_utc "nullable"
        enum status "Pending | Completed | Skipped"
        string comment
        uuid completed_by_user_id FK "nullable"
    }

    CHECKLIST_TASK_EVIDENCE {
        uuid id PK
        uuid checklist_task_execution_id FK
        string file_path
        string file_name
        string content_type
        long file_size_bytes
        timestamptz uploaded_at
        uuid uploaded_by_user_id FK
    }
```

## Notas de diseño

- **Propiedad exclusiva de `Schedule`**: no hay tabla intermedia adicional ni reuso entre
  templates/tareas. Editar el set de horarios de un template o tarea borra y recrea sus filas de
  `Schedule` (mismo patrón "reemplazar el set completo" que ya usa
  `ConfigureTemplateAssetsHandler` para `TemplateAsset`).
- **Ancla de recurrencia**: `Schedule.created_at_utc` cumple el rol de fecha de inicio para
  calcular intervalos (`Weekly` cada N semanas, `Monthly` cada N meses) — no hay un campo de
  fecha de inicio explícito en el modelo, a propósito, para no duplicar semántica con
  `created_at_utc`.
- **`Schedule.schedule_id` en `ChecklistTaskExecution` es `SET NULL` al borrar la regla**: una
  ejecución ya registrada es un hecho histórico y no debe desaparecer ni bloquear el borrado de
  una regla de horario vieja.
- **Índice único** `(template_id, asset_id, date)` en `checklist_instances`: garantiza a nivel de
  base de datos lo que antes solo validaba la aplicación (`ChecklistInstanceCreationService`),
  cerrando una condición de carrera real en generaciones concurrentes.
- **`ScheduleEvaluationService`** (`Features/ChecklistInstances/ScheduleEvaluationService.cs`) es
  el único lugar que interpreta `Schedule` para decidir "¿corresponde hoy?" — tanto el generador
  (`GenerateScheduledChecklistsHandler`) como la proyección de próximas ejecuciones
  (`GetUpcomingOccurrencesHandler`) lo reutilizan sin duplicar la lógica de fechas.
