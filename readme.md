# Prueba Técnica Angular — AudiSoft Consulting

Aplicación web CRUD para la gestión de estudiantes, profesores y notas.

## Stack

- **Backend:** ASP.NET Core 10 Web API + Entity Framework Core 10
- **Base de datos:** SQL Server Express
- **Frontend:** Angular 22 + Angular Material

## Estructura
/SchoolManagement.Domain          Entidades del negocio
/SchoolManagement.Application     DTOs, interfaces y servicios
/SchoolManagement.Infrastructure  EF Core, repositorios y migraciones
/SchoolManagement.Api             API REST
/school-management-web            Aplicación Angular
/SchoolDb_Script.sql              Script SQL generado desde las migraciones


## Requisitos previos

- .NET SDK 10
- Node.js 24 LTS
- SQL Server Express
- Angular CLI 22 (`npm install -g @angular/cli`)

## Instalación

_(pendiente)_

## Características

- CRUD completo sobre las tres entidades
- Paginación del lado del servidor
- Notificaciones de éxito y error en cada operación
- Validación de integridad referencial: no permite eliminar estudiantes o profesores con notas asociadas