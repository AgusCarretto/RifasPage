# Sistema de Gestión de Rifas - Inglaterra 2027

## 💡 Sobre el Proyecto

Este sistema fue diseñado para solucionar la gestión manual de la venta de rifas para un viaje académico. El desafío principal era manejar la concurrencia (evitar que dos personas compren el mismo número) y ofrecer transparencia en los estados de pago.

La solución implementa una **Arquitectura MVC (Modelo-Vista-Controlador)** robusta, asegurando la separación de responsabilidades y la integridad de los datos mediante transacciones en PostgreSQL.

## 🛠️ Tech Stack & Infraestructura

El proyecto utiliza tecnologías estándar de la industria enterprise:

* **Backend:** ASP.NET Core MVC (C#)
* **ORM:** Entity Framework Core (Code-First approach)
* **Base de Datos:** PostgreSQL (Railway)
* **Frontend:** Razor Views + Bootstrap + JavaScript (AJAX para validaciones)
* **Cloud:** Railway (contenedor Docker) + Brevo para el envío de mails

Guía de despliegue y variables de entorno: [docs/DEPLOY-RAILWAY.md](docs/DEPLOY-RAILWAY.md).

## 🚀 Key Features 

* **Gestión de Estados:** Lógica compleja para manejar el ciclo de vida de un número: `Disponible` -> `Reservado` -> `Vendido`.
* **Seguridad:** Autenticación y Autorización para proteger las rutas del panel administrativo.
* **Persistencia en la Nube:** Base de datos relacional PostgreSQL alojada en Railway, garantizando disponibilidad y escalabilidad.
* **Validaciones Robustas:** Verificación de datos tanto en cliente como en servidor.
