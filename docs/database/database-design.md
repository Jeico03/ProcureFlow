# ProcureFlow — Diseño de la base de datos

**Documento:** `database-design.md`  
**Versión:** 1.0  
**Estado:** Base de diseño aprobada  
**Proyecto:** ProcureFlow — Sistema integral de gestión de compras

---

## 1. Propósito

Este documento describe el diseño aprobado de la base de datos de ProcureFlow para la versión 1. Registra las entidades, atributos, relaciones, restricciones y decisiones de conservación de datos acordadas durante la Etapa 0 — Tarea 0.4: Planificación de la base de datos.

El objetivo es proporcionar una referencia estable para la implementación sin ampliar el proyecto más allá del alcance previsto.

## 2. Alcance

La base de datos dará soporte a las siguientes áreas de la aplicación:

- Gestión de usuarios y departamentos.
- Catálogo de proveedores, categorías y productos.
- Órdenes de compra y sus líneas.
- Historial de aprobaciones.
- Solicitudes de modificación y cancelación.
- Incidencias de recepción.
- Historial de auditoría.
- Archivos adjuntos.
- Notificaciones internas.
- Historial de configuración de impuestos.

El diseño de la versión 1 contiene 15 entidades de la aplicación:

1. `Role`
2. `User`
3. `Department`
4. `Supplier`
5. `Category`
6. `Product`
7. `PurchaseOrder`
8. `PurchaseOrderItem`
9. `TaxConfiguration`
10. `Approval`
11. `Request`
12. `ReceivingIssue`
13. `AuditLog`
14. `Attachment`
15. `Notification`

Las funcionalidades de gestión de inventario, almacenes, contabilidad, facturas, pagos, SaaS multiempresa, portales para proveedores, aprobaciones multinivel, recepción parcial y aplicaciones móviles quedan fuera del alcance actual.

## 3. Tecnologías y convenciones

| Área | Decisión |
|---|---|
| Base de datos relacional | Microsoft SQL Server |
| ORM | Entity Framework Core |
| Backend | ASP.NET Core Web API |
| Claves primarias | Columnas `int` con `IDENTITY` para las entidades de la aplicación |
| Texto | `nvarchar` con longitudes máximas explícitas |
| Fecha y hora | `datetime2`; las fechas de negocio sin hora usan `date` |
| Dinero y cantidades | `decimal`; nunca `float` ni `double` para valores monetarios |
| Valores booleanos | SQL Server `bit` |
| Moneda | DOP para la versión 1 |
| Almacenamiento de archivos | Los archivos se almacenan fuera de SQL Server; la base de datos conserva sus metadatos y la clave de almacenamiento |

### 3.1 Convenciones de nombres

- Los nombres de entidades y propiedades usan PascalCase singular en el modelo de dominio de C#.
- Las claves primarias usan `Id`.
- Las claves foráneas usan el nombre de la entidad referenciada seguido de `Id`, por ejemplo, `SupplierId`.
- Las propiedades booleanas usan el prefijo `Is` cuando corresponda.
- Los valores de estados y tipos se almacenan como valores controlados y son validados por el backend. También pueden utilizarse restricciones SQL cuando resulte práctico.

### 3.2 Nota sobre la implementación de Identity

El modelo de negocio requiere que cada usuario tenga un solo rol y que exista una relación directa `RoleId` desde `User` hacia `Role`.

ASP.NET Core Identity utiliza habitualmente una tabla intermedia (`AspNetUserRoles`) para asignar roles a usuarios y, de forma predeterminada, utiliza claves de tipo `string`. Por tanto, la implementación debe configurarse deliberadamente para utilizar claves enteras y respetar el modelo acordado de un único rol por usuario. No se debe introducir un segundo modelo de asignación de roles que entre en conflicto con esta decisión.

El mapeo exacto de las tablas de Identity y la implementación de autorización se definirán durante la etapa de arquitectura del backend. Esta nota no modifica la decisión de negocio aprobada: cada usuario tiene exactamente un rol.

## 4. Entidades y atributos

Los siguientes tipos son los tipos SQL Server propuestos. La nulabilidad y las restricciones forman parte del diseño, no son simples sugerencias de implementación.

### 4.1 Role

Almacena los tres roles predefinidos disponibles en la versión 1.

| Columna | Tipo SQL Server | ¿Admite NULL? | Clave / restricción | Descripción |
|---|---|---:|---|---|
| `Id` | `int` | No | PK, IDENTITY | Identificador del rol |
| `Name` | `nvarchar(50)` | No | UNIQUE | Nombre del rol |
| `Description` | `nvarchar(250)` | No | | Descripción del rol |
| `CreatedAt` | `datetime2` | No | | Fecha y hora de creación |

Roles iniciales: `Admin`, `Purchaser`, `Approver`.

Los roles son registros de la base de datos, pero la versión 1 no ofrecerá una interfaz para crear o eliminar roles. Admin puede asignar a cada usuario uno de los roles predefinidos.

### 4.2 User

Representa a una persona que puede acceder a ProcureFlow. El almacenamiento de contraseñas y la autenticación se gestionan mediante ASP.NET Core Identity.

| Columna | Tipo SQL Server | ¿Admite NULL? | Clave / restricción | Descripción |
|---|---|---:|---|---|
| `Id` | `int` | No | PK, IDENTITY | Identificador del usuario |
| `FirstName` | `nvarchar(100)` | No | | Nombre |
| `LastName` | `nvarchar(100)` | No | | Apellido |
| `Email` | `nvarchar(256)` | No | UNIQUE; unicidad normalizada | Correo de acceso y contacto |
| `PasswordHash` | Gestionado por Identity | No | | Hash de la contraseña; nunca texto plano |
| `RoleId` | `int` | No | FK → `Role.Id` | Único rol del usuario |
| `DepartmentId` | `int` | No | FK → `Department.Id` | Departamento del usuario |
| `IsActive` | `bit` | No | DEFAULT 1 | Indica si el acceso está habilitado |
| `CreatedAt` | `datetime2` | No | | Fecha y hora de creación |
| `UpdatedAt` | `datetime2` | No | | Fecha y hora de la última actualización |

Tanto el nombre como el apellido son obligatorios. El correo puede cambiarse, pero la nueva dirección debe verificarse. Desactivar un usuario bloquea su acceso sin eliminarlo ni borrar sus relaciones históricas.

### 4.3 Department

Almacena los departamentos administrados por Admin.

| Columna | Tipo SQL Server | ¿Admite NULL? | Clave / restricción | Descripción |
|---|---|---:|---|---|
| `Id` | `int` | No | PK, IDENTITY | Identificador del departamento |
| `Name` | `nvarchar(100)` | No | UNIQUE | Nombre del departamento |
| `Description` | `nvarchar(250)` | Sí | | Descripción |
| `IsActive` | `bit` | No | DEFAULT 1 | Estado activo |
| `CreatedAt` | `datetime2` | No | | Fecha y hora de creación |
| `UpdatedAt` | `datetime2` | No | | Fecha y hora de la última actualización |

Un departamento puede estar relacionado con muchos usuarios y órdenes de compra. Si un departamento está referenciado por registros históricos, se desactiva en lugar de eliminarse físicamente.

### 4.4 Supplier

Almacena la información de contacto y el estado de los proveedores.

| Columna | Tipo SQL Server | ¿Admite NULL? | Clave / restricción | Descripción |
|---|---|---:|---|---|
| `Id` | `int` | No | PK, IDENTITY | Identificador del proveedor |
| `Name` | `nvarchar(150)` | No | | Nombre del proveedor; se permiten duplicados |
| `ContactName` | `nvarchar(150)` | Sí | | Persona de contacto |
| `Email` | `nvarchar(256)` | Sí | | Correo de contacto |
| `Phone` | `nvarchar(30)` | Sí | | Teléfono de contacto |
| `Address` | `nvarchar(300)` | Sí | | Dirección del proveedor |
| `IsActive` | `bit` | No | DEFAULT 1 | Estado activo |
| `CreatedAt` | `datetime2` | No | | Fecha y hora de creación |
| `UpdatedAt` | `datetime2` | No | | Fecha y hora de la última actualización |

Se debe proporcionar al menos uno de estos datos: `Email` o `Phone`. El nombre del proveedor no es único. Si un proveedor está referenciado por órdenes de compra, se desactiva en lugar de eliminarse físicamente.

### 4.5 Category

Almacena las categorías de productos.

| Columna | Tipo SQL Server | ¿Admite NULL? | Clave / restricción | Descripción |
|---|---|---:|---|---|
| `Id` | `int` | No | PK, IDENTITY | Identificador de la categoría |
| `Name` | `nvarchar(100)` | No | UNIQUE | Nombre de la categoría |
| `Description` | `nvarchar(250)` | No | | Descripción de la categoría |
| `IsActive` | `bit` | No | DEFAULT 1 | Estado activo |
| `CreatedAt` | `datetime2` | No | | Fecha y hora de creación |
| `UpdatedAt` | `datetime2` | No | | Fecha y hora de la última actualización |

Una categoría que tenga productos relacionados se desactiva en lugar de eliminarse físicamente.

### 4.6 Product

Almacena el catálogo general de productos. En la versión 1, los productos no están vinculados a un proveedor específico.

| Columna | Tipo SQL Server | ¿Admite NULL? | Clave / restricción | Descripción |
|---|---|---:|---|---|
| `Id` | `int` | No | PK, IDENTITY | Identificador del producto |
| `SKU` | `nvarchar(50)` | No | UNIQUE | Código único del producto |
| `Name` | `nvarchar(150)` | No | | Nombre del producto |
| `Description` | `nvarchar(500)` | Sí | | Descripción del producto |
| `CategoryId` | `int` | No | FK → `Category.Id` | Categoría obligatoria |
| `UnitPrice` | `decimal(18,2)` | No | CHECK >= 0 | Precio de referencia del catálogo |
| `Status` | `nvarchar(30)` | No | Valores controlados | Disponibilidad del producto |
| `CreatedAt` | `datetime2` | No | | Fecha y hora de creación |
| `UpdatedAt` | `datetime2` | No | | Fecha y hora de la última actualización |

Estados permitidos: `Active`, `OutOfStock`, `Discontinued`.

El `SKU` debe seguir siendo único incluso si el producto está inactivo o descontinuado. Los productos con referencias históricas se desactivan, no se eliminan físicamente. Un producto que no esté disponible para nuevas compras no puede agregarse a una nueva orden.

### 4.7 PurchaseOrder

Representa una orden de compra y su estado actual dentro del flujo de trabajo.

| Columna | Tipo SQL Server | ¿Admite NULL? | Clave / restricción | Descripción |
|---|---|---:|---|---|
| `Id` | `int` | No | PK, IDENTITY | Identificador de la orden |
| `OrderNumber` | `nvarchar(30)` | No | UNIQUE | Número comercial generado |
| `SupplierId` | `int` | No | FK → `Supplier.Id` | Proveedor obligatorio |
| `DepartmentId` | `int` | No | FK → `Department.Id` | Departamento solicitante |
| `CreatedByUserId` | `int` | No | FK → `User.Id` | Usuario que creó originalmente la orden |
| `AssignedToUserId` | `int` | No | FK → `User.Id` | Responsable operativo actual |
| `Status` | `nvarchar(30)` | No | Valores controlados | Estado actual de la orden |
| `Justification` | `nvarchar(500)` | No al enviar | | Justificación de negocio |
| `ExpectedDeliveryDate` | `date` | No al enviar | | Fecha estimada de entrega |
| `Subtotal` | `decimal(18,2)` | No | CHECK >= 0 | Suma antes de impuestos |
| `TaxAmount` | `decimal(18,2)` | No | CHECK >= 0 | Total de impuestos |
| `TotalAmount` | `decimal(18,2)` | No | CHECK >= 0 | Total con impuestos incluidos |
| `CreatedAt` | `datetime2` | No | | Fecha y hora de creación |
| `UpdatedAt` | `datetime2` | No | | Fecha y hora de la última actualización |

Estados permitidos: `Draft`, `PendingApproval`, `Approved`, `Rejected`, `ReceivingIssue`, `Received`, `Cancelled`.

`OrderNumber` sigue un formato generado como `PO-2026-000001`. La generación del número debe ser segura ante operaciones concurrentes; no debe depender de un cálculo inseguro como `MAX(Id) + 1`.

El creador y el responsable asignado son referencias distintas. Inicialmente, el creador queda asignado a la orden. Una reasignación no modifica quién creó originalmente la orden.

El proveedor y el departamento son obligatorios desde la creación. La justificación y la fecha estimada de entrega son obligatorias antes de enviar la orden para aprobación. Una orden debe contener al menos una línea desde su creación y nunca debe quedar sin líneas.

Las órdenes no se eliminan físicamente.

### 4.8 PurchaseOrderItem

Representa una línea de producto dentro de una orden de compra y conserva los datos históricos del producto utilizados por esa orden.

| Columna | Tipo SQL Server | ¿Admite NULL? | Clave / restricción | Descripción |
|---|---|---:|---|---|
| `Id` | `int` | No | PK, IDENTITY | Identificador de la línea |
| `PurchaseOrderId` | `int` | No | FK → `PurchaseOrder.Id` | Orden a la que pertenece |
| `ProductId` | `int` | No | FK → `Product.Id` | Referencia al producto del catálogo |
| `ProductName` | `nvarchar(150)` | No | | Copia histórica del nombre del producto |
| `SKU` | `nvarchar(50)` | No | | Copia histórica del SKU |
| `Quantity` | `decimal(18,2)` | No | CHECK > 0 | Cantidad solicitada |
| `UnitPrice` | `decimal(18,2)` | No | CHECK >= 0 | Precio unitario histórico |
| `TaxRate` | `decimal(5,2)` | No | CHECK dentro del rango configurado | Tasa de impuesto histórica |
| `TaxAmount` | `decimal(18,2)` | No | CHECK >= 0 | Impuesto de la línea |
| `LineTotal` | `decimal(18,2)` | No | CHECK >= 0 | Total de la línea |

Las cantidades pueden ser fraccionarias. El backend calcula los importes de las líneas y los totales de la orden. El nombre del producto, el SKU, el precio unitario y la tasa de impuesto son copias históricas y no deben cambiar cuando se actualicen los datos del catálogo o la configuración de impuestos.

Una línea solo puede eliminarse de una orden `Draft` si la orden conserva al menos una línea. La aplicación debe garantizar esta regla dentro de una transacción.

### 4.9 TaxConfiguration

Almacena la configuración de impuestos versionada. Cada cambio crea un nuevo registro de configuración; las configuraciones anteriores se conservan.

| Columna | Tipo SQL Server | ¿Admite NULL? | Clave / restricción | Descripción |
|---|---|---:|---|---|
| `Id` | `int` | No | PK, IDENTITY | Identificador de la configuración |
| `TaxName` | `nvarchar(100)` | No | | Nombre del impuesto |
| `TaxRate` | `decimal(5,2)` | No | CHECK dentro del rango configurado | Porcentaje del impuesto |
| `IsActive` | `bit` | No | Índice único filtrado | Indica la configuración vigente |
| `UpdatedByUserId` | `int` | No | FK → `User.Id` | Admin que creó o modificó la configuración |
| `UpdatedAt` | `datetime2` | No | | Fecha y hora de la configuración |

Solo puede existir una configuración activa a la vez. La base de datos debe garantizarlo mediante un índice único filtrado. Las tasas históricas se conservan, mientras que las líneas de las órdenes almacenan la tasa que realmente se les aplicó.

### 4.10 Approval

Almacena cada intento de aprobación de una orden. Cada nuevo envío crea un registro; las decisiones anteriores se conservan.

| Columna | Tipo SQL Server | ¿Admite NULL? | Clave / restricción | Descripción |
|---|---|---:|---|---|
| `Id` | `int` | No | PK, IDENTITY | Identificador del registro de aprobación |
| `PurchaseOrderId` | `int` | No | FK → `PurchaseOrder.Id` | Orden relacionada |
| `ApproverUserId` | `int` | No | FK → `User.Id` | Aprobador asignado |
| `Decision` | `nvarchar(30)` | No | Valores controlados | `Pending`, `Approved`, `Rejected` |
| `Comments` | `nvarchar(500)` | Sí | | Comentarios del aprobador |
| `CreatedAt` | `datetime2` | No | | Fecha y hora de creación del registro |
| `DecidedAt` | `datetime2` | Sí | | Fecha y hora de la decisión |

Una aprobación pendiente no tiene `DecidedAt`. Una aprobación completada sí tiene una fecha de decisión. El historial de aprobaciones nunca se elimina.

### 4.11 Request

Almacena solicitudes de modificación y cancelación asociadas a órdenes de compra.

| Columna | Tipo SQL Server | ¿Admite NULL? | Clave / restricción | Descripción |
|---|---|---:|---|---|
| `Id` | `int` | No | PK, IDENTITY | Identificador de la solicitud |
| `PurchaseOrderId` | `int` | No | FK → `PurchaseOrder.Id` | Orden relacionada |
| `CreatedByUserId` | `int` | No | FK → `User.Id` | Usuario que creó la solicitud |
| `ReviewedByUserId` | `int` | Sí | FK → `User.Id` | Admin que revisó la solicitud |
| `Type` | `nvarchar(30)` | No | `Modification`, `Cancellation` | Tipo de solicitud |
| `Status` | `nvarchar(30)` | No | `Pending`, `Approved`, `Rejected` | Estado de la solicitud |
| `Reason` | `nvarchar(500)` | No | | Motivo de la solicitud |
| `Description` | `nvarchar(1000)` | Sí | | Detalles adicionales |
| `CreatedAt` | `datetime2` | No | | Fecha y hora de creación |
| `ReviewedAt` | `datetime2` | Sí | | Fecha y hora de revisión |

Una solicitud pendiente no tiene revisor ni fecha de revisión. Las solicitudes se conservan como registros históricos. Las solicitudes generales y una entidad separada para comentarios o conversaciones quedan fuera de la versión 1.

### 4.12 ReceivingIssue

Almacena las incidencias identificadas durante la recepción de una orden de compra. Cada incidencia identifica una línea específica de la orden.

| Columna | Tipo SQL Server | ¿Admite NULL? | Clave / restricción | Descripción |
|---|---|---:|---|---|
| `Id` | `int` | No | PK, IDENTITY | Identificador de la incidencia |
| `PurchaseOrderId` | `int` | No | FK → `PurchaseOrder.Id` | Orden relacionada |
| `PurchaseOrderItemId` | `int` | No | FK compuesta con `PurchaseOrderId` | Línea de la orden afectada |
| `ReportedByUserId` | `int` | No | FK → `User.Id` | Usuario que reportó la incidencia |
| `ResolvedByUserId` | `int` | Sí | FK → `User.Id` | Usuario que resolvió la incidencia |
| `Type` | `nvarchar(30)` | No | `Damage`, `MissingQuantity`, `Other` | Tipo de incidencia |
| `Description` | `nvarchar(1000)` | No | | Descripción de la incidencia |
| `Status` | `nvarchar(30)` | No | `Open`, `Resolved` | Estado de la incidencia |
| `ResolutionNotes` | `nvarchar(1000)` | Sí | | Detalles de la resolución |
| `CreatedAt` | `datetime2` | No | | Fecha y hora del reporte |
| `ResolvedAt` | `datetime2` | Sí | | Fecha y hora de resolución |

`ResolvedByUserId` y `ResolvedAt` son nulos mientras la incidencia esté abierta. La clave foránea compuesta debe garantizar que el `PurchaseOrderItem` seleccionado pertenezca al mismo `PurchaseOrder` referenciado por la incidencia.

Una orden permanece en estado `ReceivingIssue` hasta que todas las incidencias estén resueltas y la entrega esté completa y en buenas condiciones.

### 4.13 AuditLog

Almacena eventos importantes del negocio y de seguridad.

| Columna | Tipo SQL Server | ¿Admite NULL? | Clave / restricción | Descripción |
|---|---|---:|---|---|
| `Id` | `int` | No | PK, IDENTITY | Identificador del evento de auditoría |
| `UserId` | `int` | Sí | FK → `User.Id` | Actor; nulo para eventos del sistema |
| `Action` | `nvarchar(100)` | No | | Nombre de la acción o evento |
| `EntityType` | `nvarchar(50)` | No | | Tipo de entidad afectada |
| `EntityId` | `int` | No | | Identificador del registro afectado |
| `Details` | `nvarchar(max)` | Sí | | Detalles adicionales del evento |
| `CreatedAt` | `datetime2` | No | | Fecha y hora del evento |

`EntityType` y `EntityId` forman una referencia polimórfica. SQL Server no puede aplicar una clave foránea normal entre varias tablas posibles, por lo que el backend valida los tipos de entidad admitidos y sus referencias cuando corresponda. Los registros de auditoría se conservan.

### 4.14 Attachment

Almacena los metadatos de archivos asociados exactamente a una orden de compra, solicitud o incidencia de recepción. El contenido de los archivos se almacena fuera de SQL Server.

| Columna | Tipo SQL Server | ¿Admite NULL? | Clave / restricción | Descripción |
|---|---|---:|---|---|
| `Id` | `int` | No | PK, IDENTITY | Identificador del archivo adjunto |
| `PurchaseOrderId` | `int` | Sí | FK → `PurchaseOrder.Id` | Destino opcional: orden |
| `RequestId` | `int` | Sí | FK → `Request.Id` | Destino opcional: solicitud |
| `ReceivingIssueId` | `int` | Sí | FK → `ReceivingIssue.Id` | Destino opcional: incidencia |
| `UploadedByUserId` | `int` | No | FK → `User.Id` | Usuario que subió el archivo |
| `FileName` | `nvarchar(255)` | No | | Nombre original del archivo |
| `StorageKey` | `nvarchar(500)` | No | UNIQUE | Identificador del archivo en el almacenamiento |
| `ContentType` | `nvarchar(100)` | No | | Tipo MIME |
| `FileSize` | `bigint` | No | CHECK >= 0 | Tamaño en bytes |
| `IsDeleted` | `bit` | No | DEFAULT 0 | Indicador de eliminación lógica |
| `UploadedAt` | `datetime2` | No | | Fecha y hora de carga |
| `DeletedAt` | `datetime2` | Sí | | Fecha y hora de eliminación lógica |

Una restricción CHECK debe garantizar que exactamente uno de estos campos tenga valor: `PurchaseOrderId`, `RequestId` o `ReceivingIssueId`. La eliminación lógica oculta un archivo adjunto sin borrar automáticamente sus metadatos históricos. La limpieza física del archivo es una operación independiente del almacenamiento y debe seguir la política de conservación.

### 4.15 Notification

Almacena las notificaciones internas de los usuarios.

| Columna | Tipo SQL Server | ¿Admite NULL? | Clave / restricción | Descripción |
|---|---|---:|---|---|
| `Id` | `int` | No | PK, IDENTITY | Identificador de la notificación |
| `RecipientUserId` | `int` | No | FK → `User.Id` | Usuario destinatario |
| `Type` | `nvarchar(50)` | No | | Tipo de notificación |
| `Title` | `nvarchar(150)` | No | | Título de la notificación |
| `Message` | `nvarchar(500)` | No | | Mensaje de la notificación |
| `RelatedEntityType` | `nvarchar(50)` | Sí | | Tipo de entidad relacionada |
| `RelatedEntityId` | `int` | Sí | | Identificador del registro relacionado |
| `IsRead` | `bit` | No | DEFAULT 0 | Estado de lectura |
| `IsDeleted` | `bit` | No | DEFAULT 0 | Indica si está oculta de la bandeja |
| `CreatedAt` | `datetime2` | No | | Fecha y hora de creación |
| `ReadAt` | `datetime2` | Sí | | Fecha y hora de lectura |
| `DeletedAt` | `datetime2` | Sí | | Fecha y hora en que se ocultó |

En la versión 1, las notificaciones son internas de la aplicación. Los usuarios pueden ocultarlas de su bandeja sin eliminar físicamente los registros históricos. `RelatedEntityType` y `RelatedEntityId` forman una referencia polimórfica que valida el backend.

## 5. Relaciones y cardinalidades

| Entidad principal | Entidad relacionada | Cardinalidad | Regla |
|---|---|---|---|
| Role | User | 1:N | Cada usuario tiene exactamente un rol |
| Department | User | 1:N | Cada usuario pertenece a un departamento |
| Category | Product | 1:N | Cada producto tiene una categoría |
| Supplier | PurchaseOrder | 1:N | Cada orden tiene un proveedor |
| Department | PurchaseOrder | 1:N | Cada orden tiene un departamento |
| User | PurchaseOrder (creador) | 1:N | Cada orden tiene un creador original |
| User | PurchaseOrder (asignado) | 1:N | Cada orden tiene un responsable actual |
| PurchaseOrder | PurchaseOrderItem | 1:N | Cada orden tiene al menos una línea |
| Product | PurchaseOrderItem | 1:N | Cada línea referencia un producto del catálogo |
| User | TaxConfiguration | 1:N | Cada configuración registra al Admin responsable |
| PurchaseOrder | Approval | 1:N | Una orden puede tener varios intentos de aprobación |
| User | Approval | 1:N | Cada aprobación identifica a su aprobador |
| PurchaseOrder | Request | 1:N | Cada solicitud pertenece a una orden |
| User | Request (creador) | 1:N | Cada solicitud tiene un creador |
| User | Request (revisor) | 1:N | Una solicitud puede tener un revisor |
| PurchaseOrder | ReceivingIssue | 1:N | Cada incidencia pertenece a una orden |
| PurchaseOrderItem | ReceivingIssue | 1:N | Cada incidencia identifica una línea afectada |
| User | ReceivingIssue (reportador) | 1:N | Cada incidencia tiene un reportador |
| User | ReceivingIssue (resolutor) | 1:N | Una incidencia puede tener un responsable de resolución |
| User | AuditLog | 1:N | El actor de auditoría puede ser nulo en eventos del sistema |
| PurchaseOrder | Attachment | 1:N | Destino polimórfico opcional |
| Request | Attachment | 1:N | Destino polimórfico opcional |
| ReceivingIssue | Attachment | 1:N | Destino polimórfico opcional |
| User | Attachment | 1:N | Cada archivo adjunto tiene un usuario que lo subió |
| User | Notification | 1:N | Cada notificación tiene un destinatario |

`TaxConfiguration`, `AuditLog` y `Notification` no requieren relaciones directas con cada registro que puedan describir. Cuando se utilicen referencias polimórficas, la validación se realizará en la aplicación.

## 6. Índices y unicidad

Como mínimo, se deben implementar los siguientes índices:

| Tabla | Índice / restricción | Propósito |
|---|---|---|
| `Role` | UNIQUE (`Name`) | Evitar roles duplicados |
| `User` | Correo normalizado UNIQUE | Garantizar la unicidad del correo de forma consistente |
| `Department` | UNIQUE (`Name`) | Evitar departamentos duplicados |
| `Category` | UNIQUE (`Name`) | Evitar categorías duplicadas |
| `Product` | UNIQUE (`SKU`) | Evitar SKU duplicados |
| `PurchaseOrder` | UNIQUE (`OrderNumber`) | Evitar números de orden duplicados |
| `PurchaseOrderItem` | UNIQUE (`PurchaseOrderId`, `Id`) | Permitir la FK compuesta desde ReceivingIssue |
| `TaxConfiguration` | Índice único filtrado para el registro activo | Permitir una sola configuración activa |
| `Attachment` | UNIQUE (`StorageKey`) | Evitar identificadores de almacenamiento duplicados |

Se deben agregar índices a las claves foráneas y a las rutas de filtrado habituales según los patrones reales de consulta. Evita crear índices indiscriminadamente; las necesidades de rendimiento se validarán a medida que avance la aplicación.

## 7. Restricciones de integridad y reglas de negocio

### 7.1 Restricciones numéricas

- `PurchaseOrderItem.Quantity > 0`.
- Los precios unitarios de productos y líneas de órdenes deben ser `>= 0`.
- Las tasas de impuestos y los importes monetarios no pueden ser negativos.
- Las tasas de impuestos deben estar dentro del rango definido para el modelo de impuestos de la aplicación.
- Los cálculos del backend son la fuente de verdad; no se deben aceptar como confiables los totales enviados por el cliente.
- Los valores monetarios utilizan `decimal` en C# y SQL Server.

### 7.2 Integridad de las órdenes

- El proveedor y el departamento son obligatorios al crear una orden.
- Una orden tiene al menos una línea desde su creación.
- Una orden no puede quedarse sin líneas cuando se elimina una de ellas.
- La justificación y la fecha estimada de entrega son obligatorias antes del envío.
- Una orden aprobada no se edita directamente; se debe utilizar el flujo de solicitud de modificación.
- El creador puede corregir una orden rechazada y volver a enviarla al flujo de aprobación.
- Una orden solo se marca como `Received` cuando todos los productos y cantidades se reciben completos y en buenas condiciones.
- Una orden con incidencias sin resolver permanece en estado `ReceivingIssue`.

La regla que exige al menos una línea hija no puede garantizarse únicamente mediante una clave foránea normal. Debe aplicarse en una operación transaccional de la aplicación y, si es necesario, pueden evaluarse protecciones adicionales en la base de datos.

### 7.3 Copias históricas

`PurchaseOrderItem` almacena el nombre del producto, SKU, precio unitario, tasa de impuesto e importes calculados de la línea como copias históricas. Los cambios en el catálogo de productos o en la configuración actual de impuestos no deben modificar el historial de órdenes existentes.

### 7.4 Comportamiento de eliminación

No se debe utilizar eliminación en cascada de forma indiscriminada.

- Los usuarios con historial de negocio se desactivan; no se eliminan físicamente.
- Los proveedores, productos, categorías y departamentos referenciados por registros históricos se desactivan.
- Las órdenes de compra, aprobaciones, solicitudes, incidencias de recepción y registros de auditoría se conservan.
- Las líneas de órdenes solo pueden eliminarse de una orden `Draft` cuando quede al menos una línea.
- Los archivos adjuntos y las notificaciones utilizan eliminación lógica.
- El comportamiento de eliminación de cada clave foránea debe configurarse explícitamente, restringiendo generalmente la eliminación de registros referenciados por datos históricos.

## 8. Valores de estados y tipos

| Entidad | Campo | Valores permitidos en v1 |
|---|---|---|
| Product | `Status` | `Active`, `OutOfStock`, `Discontinued` |
| PurchaseOrder | `Status` | `Draft`, `PendingApproval`, `Approved`, `Rejected`, `ReceivingIssue`, `Received`, `Cancelled` |
| Approval | `Decision` | `Pending`, `Approved`, `Rejected` |
| Request | `Type` | `Modification`, `Cancellation` |
| Request | `Status` | `Pending`, `Approved`, `Rejected` |
| ReceivingIssue | `Type` | `Damage`, `MissingQuantity`, `Other` |
| ReceivingIssue | `Status` | `Open`, `Resolved` |

Las transiciones de estado y los permisos por rol se aplican mediante la lógica de negocio del backend, no únicamente desde Angular.

## 9. Notas de implementación y puntos por finalizar durante la arquitectura del backend

Estos detalles no cambian el modelo de negocio aprobado, pero deben resolverse antes o durante la implementación:

1. **Mapeo de ASP.NET Core Identity:** configurar claves enteras e implementar de forma consistente el modelo de un solo rol. Evitar combinar la asignación directa mediante `RoleId` con una asignación muchos-a-muchos de roles sin restricciones.
2. **Precisión y rango de la tasa de impuesto:** confirmar si las tasas se representan como porcentajes (por ejemplo, `18.00`) o como fracciones (por ejemplo, `0.18`) y aplicar una sola convención de forma consistente.
3. **Historial de configuración de impuestos:** tratar cada cambio como una nueva versión; definir la transacción exacta de creación/desactivación y el índice único filtrado.
4. **Generación del número de orden:** utilizar una secuencia segura ante operaciones concurrentes o un mecanismo equivalente.
5. **FK compuesta de ReceivingIssue:** agregar una clave candidata única en `PurchaseOrderItem(PurchaseOrderId, Id)` y referenciarla desde `ReceivingIssue(PurchaseOrderId, PurchaseOrderItemId)`.
6. **Referencias polimórficas:** validar en la aplicación la semántica de destino de `AuditLog`, `Notification` y `Attachment`, junto con una restricción CHECK para garantizar que Attachment tenga exactamente un destino.
7. **Ciclo de vida del almacenamiento de archivos:** definir validación segura de cargas, control de acceso, límites de tamaño y comportamiento de limpieza para archivos eliminados lógicamente.
8. **Actor de auditoría:** los eventos generados por el sistema pueden tener `UserId` nulo; se deben registrar suficientes detalles para explicar la acción del sistema.
9. **Nulabilidad de registros de flujo de trabajo:** `Request.ReviewedByUserId` y `ReceivingIssue.ResolvedByUserId` permanecen nulos hasta que se realice la revisión o resolución.
10. **Marcas de tiempo:** almacenar las marcas de tiempo de forma consistente (preferiblemente UTC en la aplicación) y convertirlas para su visualización.

## 10. Aprobación y versionado

Este documento registra la base de diseño aprobada de la base de datos de ProcureFlow para la versión 1.

El archivo fuente del ERD se mantiene por separado como `ProcureFlow-ERD-v1.mmd`.

Los cambios en entidades, relaciones o restricciones de negocio deben revisarse según el alcance del proyecto y actualizarse tanto en este documento como en el ERD. Los detalles menores de implementación pueden ajustarse sin ampliar el alcance de negocio aprobado.
