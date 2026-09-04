# Prueba Técnica — AudiSoft Consulting

Aplicación web para la gestión de estudiantes, profesores y notas, con operaciones
CRUD, control de acceso por roles y validación de integridad referencial.

## Stack

- **Backend:** ASP.NET Core 10 Web API + Entity Framework Core 10
- **Base de datos:** SQL Server Express
- **Frontend:** Angular 22 + Angular Material
- **Autenticación:** Keycloak 26.7 (OpenID Connect) sobre Docker

## Arquitectura

El backend sigue Clean Architecture en cuatro proyectos. Las dependencias apuntan
hacia el núcleo: `Domain` no referencia ningún paquete externo.

```
/backend
  /SchoolManagement.Domain          Entidades del negocio
  /SchoolManagement.Application     DTOs, interfaces, servicios y excepciones
  /SchoolManagement.Infrastructure  EF Core, repositorios y migraciones
  /SchoolManagement.Api             Controladores, middleware y configuración
  SchoolDb_Script.sql               Script SQL generado desde las migraciones
/frontend
  /school-management-web            Aplicación Angular
/keycloak
  /import                           Realm exportado (se importa automáticamente)
/docs                               Documentación y capturas
docker-compose.yml                  Servicio de Keycloak
```

## Modelo de datos

Tres tablas. `Grades` es la tabla puente, con dos llaves foráneas en
`ON DELETE NO ACTION` para impedir el borrado de registros referenciados.

| Tabla | Columnas |
|---|---|
| Students | Id, Name |
| Teachers | Id, Name |
| Grades | Id, Name, StudentId (FK), TeacherId (FK), Value |

`Value` es `decimal(4,2)` con un `CHECK` que restringe el rango a 0.0 – 5.0.

## Requisitos previos

- .NET SDK 10
- Node.js 24 LTS
- SQL Server Express
- Docker Desktop
- Angular CLI 22 (`npm install -g @angular/cli`)

## Instalación

### 1. Base de datos

Desde `/backend`, las migraciones crean la base desde cero:

```bash
dotnet ef database update -p SchoolManagement.Infrastructure -s SchoolManagement.Api
```

Si la instancia de SQL Server no es `localhost\SQLEXPRESS`, ajustar
`ConnectionStrings:DefaultConnection` en `SchoolManagement.Api/appsettings.json`.

Alternativa: ejecutar `SchoolDb_Script.sql` directamente en SSMS.

### 2. Keycloak

Desde la raíz del repositorio:

```bash
docker compose up -d
```

Keycloak queda disponible en http://localhost:8080 (admin / admin). El realm
`school-management` se importa automáticamente con sus clientes y roles.

Los usuarios deben crearse manualmente (ver sección siguiente).

### 3. API

```bash
cd backend
dotnet run --project SchoolManagement.Api
```

Queda en http://localhost:5160. Swagger en http://localhost:5160/swagger

### 4. Frontend

```bash
cd frontend/school-management-web
npm install
ng serve
```

Queda en http://localhost:4200

## Usuarios de prueba

Keycloak no exporta credenciales, así que los usuarios deben crearse a mano en
http://localhost:8080 → admin/admin → realm `school-management` → Users → Add user.

| Usuario | Contraseña | Rol |
|---|---|---|
| admin.school | Admin123! | administrator |
| profesor.school | Profesor123! | teacher |
| estudiante.school | Estudiante123! | student |

Al crearlos: **First name** y **Last name** son obligatorios (Keycloak rechaza el
login si falta alguno), **Email verified** en On, y al asignar la contraseña dejar
**Temporary** en Off. El rol se asigna en la pestaña Role mapping, filtrando por
realm roles.

## Permisos por rol

| Rol | Estudiantes | Profesores | Notas |
|---|---|---|---|
| administrator | CRUD | CRUD | Lectura |
| teacher | Lectura | Lectura | CRUD |
| student | Lectura | Lectura | Lectura |

La autorización se aplica en dos niveles: la interfaz oculta las acciones no
permitidas, y la API las rechaza con 403 mediante políticas basadas en los roles
del token, de modo que el control no puede eludirse desde el cliente.

## Características

- CRUD completo sobre las tres entidades
- Paginación y ordenamiento del lado del servidor
- Búsqueda con debounce sobre nombre, estudiante y profesor
- Notificaciones de éxito y error en cada operación
- Validación de formularios en cliente y servidor
- Integridad referencial: no permite eliminar estudiantes o profesores con notas
  asociadas, devolviendo 409 con un mensaje que indica cuántas notas lo impiden
- Manejo centralizado de errores mediante middleware e interceptor HTTP
- Autenticación OpenID Connect con Authorization Code Flow + PKCE

## Consideraciones de seguridad

Esta es una configuración de desarrollo. En un entorno productivo habría que:

- Ejecutar Keycloak con `start` y PostgreSQL en lugar de `start-dev` con H2
- Servir Keycloak y la API sobre HTTPS (`RequireHttpsMetadata: true`)
- Sustituir el usuario administrador temporal de Keycloak por uno permanente
- Externalizar credenciales a variables de entorno o un gestor de secretos