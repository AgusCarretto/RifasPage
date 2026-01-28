# Sistema de Gestión de Rifas - Inglaterra 2027

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

## 🚀 Key Features 

* **Gestión de Estados:** Lógica compleja para manejar el ciclo de vida de un número: `Disponible` -> `Reservado` -> `Vendido`.
* **Seguridad:** Autenticación y Autorización para proteger las rutas del panel administrativo.
* **Persistencia en la Nube:** Base de datos relacional alojada en Azure SQL, garantizando disponibilidad y escalabilidad.
* **Validaciones Robustas:** Verificación de datos tanto en cliente como en servidor.
