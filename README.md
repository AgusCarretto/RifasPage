# 🇬🇧 Sistema de Gestión de Rifas - Londres 2027

![Banner Rifas](https://socialify.git.ci/AgusCarretto/sistema-rifas/image?description=1&font=KoHo&language=1&name=1&owner=1&pattern=Formal&theme=Light)

> Plataforma Full-Stack desarrollada en .NET para la gestión integral de recaudación de fondos. Incluye venta pública de números, pasarela de reserva y un panel administrativo avanzado para auditoría de pagos.

![Azure Status](https://img.shields.io/badge/Deploy-Azure_Cloud-0078D4?style=for-the-badge&logo=microsoft-azure&logoColor=white)
![.NET Status](https://img.shields.io/badge/Backend-.NET_Core_MVC-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

## 💡 Sobre el Proyecto

Este sistema fue diseñado para solucionar la gestión manual de la venta de rifas para un viaje académico. El desafío principal era manejar la concurrencia (evitar que dos personas compren el mismo número) y ofrecer transparencia en los estados de pago.

La solución implementa una **Arquitectura MVC (Modelo-Vista-Controlador)** robusta, asegurando la separación de responsabilidades y la integridad de los datos mediante transacciones en SQL Server.

## 🛠️ Tech Stack & Infraestructura

El proyecto utiliza tecnologías estándar de la industria enterprise:

* **Backend:** ASP.NET Core MVC (C#)
* **ORM:** Entity Framework Core (Code-First approach)
* **Base de Datos:** Azure SQL Database (SQL Server)
* **Frontend:** Razor Views + Bootstrap + JavaScript (AJAX para validaciones)
* **Cloud:** Azure App Service

## 📸 Funcionalidades y Capturas

### 1. Panel de Administración (Back-Office)
*Acceso restringido para administradores. Permite visualizar métricas en tiempo real y cambiar estados de pago.*

| Dashboard General | Gestión de Usuarios |
|:-----------------:|:-------------------:|
| ![Dashboard](link-a-tu-foto-admin-dashboard.png) | ![Tabla](link-a-tu-foto-tabla-admin.png) |
| *Control de recaudación total* | *Validación de pagos y reservas* |

### 2. Experiencia de Usuario (Frontend)
*Diseño Mobile-First para facilitar la compra rápida desde celulares.*

| Selección de Números | Modal de Compra |
|:--------------------:|:---------------:|
| ![Grilla](link-a-tu-foto-celular-grilla.png) | ![Modal](link-a-tu-foto-modal.png) |
| *Grilla interactiva de disponibilidad* | *Formulario de reserva* |

## 🚀 Key Features (Lo que hace especial al código)

* **Gestión de Estados:** Lógica compleja para manejar el ciclo de vida de un número: `Disponible` -> `Reservado` -> `Confirmado` -> `Vendido`.
* **Seguridad:** Autenticación y Autorización para proteger las rutas del panel administrativo.
* **Persistencia en la Nube:** Base de datos relacional alojada en Azure SQL, garantizando disponibilidad y escalabilidad.
* **Validaciones Robustas:** Verificación de datos tanto en cliente (JS) como en servidor (Data Annotations en C#).

## 🔧 Configuración Local

Si deseas clonar y correr este proyecto:

1.  **Requisitos:** .NET SDK 8.0+ y SQL Server Local.
2.  **Configuración:** Actualizar la `ConnectionString` en `appsettings.json`.
3.  **Migraciones:**
    ```bash
    dotnet ef database update
    ```
4.  **Ejecutar:**
    ```bash
    dotnet run
    ```

## 📂 Estructura MVC

El código sigue las buenas prácticas de organización de Microsoft:

```text
src/
├── Controllers/   # Lógica de negocio (AdminController, RifasController)
├── Models/        # Definición de entidades (User, Rifa, Payment)
├── Views/         # Interfaz de usuario (Razor)
└── Data/          # Contexto de base de datos (DbContext)
