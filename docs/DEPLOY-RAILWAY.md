# Despliegue en Railway

La app corre en Railway como un contenedor (`Dockerfile` en la raíz) con una base PostgreSQL de Railway.
Los mails se envían por la API HTTPS de [Brevo](https://www.brevo.com), porque Railway bloquea el SMTP saliente
(puertos 25, 465, 587 y 2525) en el plan Hobby.

## Variables de entorno del servicio web

| Variable | Valor | Obligatoria |
|---|---|---|
| `DATABASE_URL` | `${{Postgres.DATABASE_URL}}` (referencia al servicio de base de datos; ajustá `Postgres` al nombre real del servicio) | Sí |
| `EmailSettings__ApiKey` | API key de Brevo | Sí (sin esto no salen mails, pero la reserva igual se guarda y el error queda en los logs) |
| `EmailSettings__SenderEmail` | Remitente verificado en Brevo | Sí |
| `EmailSettings__AdminEmail` | Mail donde llega el aviso de cada reserva | Sí |
| `EmailSettings__SenderName` | Nombre del remitente (por defecto `Rifas Londres 2027`) | No |

El `Dockerfile` ya fija `ASPNETCORE_ENVIRONMENT=Production`, `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true` y
`Database__MigrateOnStartup=true` (las migraciones de EF se aplican solas al arrancar). `PORT` lo inyecta Railway.

Si preferís un connection string de Npgsql en vez de `DATABASE_URL`, definí `ConnectionStrings__DefaultConnection`
(tiene prioridad).

## Pasos

### 0. Antes de que se cierre Azure
1. Exportá los datos: ejecutá [`deploy/azure-export.sql`](../deploy/azure-export.sql) en Azure SQL y guardá el resultado como `import.sql`.
   Si exportaste la celda como JSON (`results.json`), extraé el SQL con PowerShell (sin BOM, para que `psql` lo lea bien):
   ```powershell
   $j = Get-Content results.json -Raw -Encoding UTF8 | ConvertFrom-Json
   [IO.File]::WriteAllText("$PWD\import.sql", $j[0].postgres_import_sql, (New-Object Text.UTF8Encoding $false))
   ```
   `import.sql` y `results.json` tienen datos personales de los compradores y la clave del admin: no los guardes dentro del repo.
2. Copiá la configuración del App Service (connection string y cualquier otra variable) por si hace falta consultarla.

### 1. Mails (Brevo)
1. Creá una cuenta en Brevo y verificá el remitente que vas a usar (`EmailSettings__SenderEmail`).
2. Generá una API key (sección *SMTP & API*).
3. Si el remitente es una casilla de Gmail, probá que los mails no caigan en spam; un dominio propio verificado mejora mucho la entregabilidad.

### 2. Railway
1. *New Project* > *Deploy from GitHub repo* > elegí el repo.
2. En el servicio web: *Settings* > *Source* > rama `develop` (después se puede cambiar a `master`).
3. *New* > *Database* > *PostgreSQL*.
4. Cargá las variables de la tabla de arriba en el servicio web.
5. *Settings* > *Networking* > *Generate Domain*. Opcional: *Settings* > *Deploy* > *Healthcheck Path* = `/healthz`.
6. Deploy. Al arrancar, la app crea las tablas (logs: `CREATE TABLE "Rifas"...`).

### 3. Importar los datos
Con la app ya desplegada (las tablas existen y están vacías):

```bash
psql "<DATABASE_PUBLIC_URL del servicio Postgres>" -v ON_ERROR_STOP=1 -f import.sql
```

La URL pública está en el servicio Postgres > *Variables* (`DATABASE_PUBLIC_URL`) o en la pestaña *Connect*.
El script va dentro de una transacción: si falla, no queda nada a medias. Si no tenías datos para exportar,
creá el admin a mano:

```sql
INSERT INTO "Admin" ("name","password","email") VALUES ('<usuario>','<clave>','<mail>');
```

y usá el botón de cargar las 80 rifas del dashboard (o insertalas con SQL).

### 4. Probar
- `/healthz` responde `ok`.
- La home lista las rifas.
- Login en `/Admin/Login` y dashboard.
- Reservá una rifa de prueba: tienen que llegar el mail al comprador y el aviso al admin. Si no llegan, revisá los logs
  del servicio (`No se pudo enviar el mail de la reserva ...` trae el motivo que devuelve Brevo).
- Cancelá la reserva de prueba desde el dashboard.

## Desarrollo local
Hace falta un PostgreSQL local. `appsettings.Development.json` apunta a
`Host=localhost;Port=5432;Database=rifas;Username=postgres;Password=postgres`; si tu instancia usa otras credenciales,
sobrescribilas con `dotnet user-secrets` o con la variable `ConnectionStrings__DefaultConnection`.

```bash
# Postgres descartable con Docker (opcional)
docker run --name rifas-pg -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=rifas -p 5432:5432 -d postgres:16

# Aplicar migraciones
dotnet ef database update --project LogicaAccesoDatos --startup-project MVC

# Mails en local: user-secrets (no se commitean)
dotnet user-secrets init --project MVC
dotnet user-secrets set "EmailSettings:ApiKey" "<api key>" --project MVC
dotnet user-secrets set "EmailSettings:SenderEmail" "<remitente>" --project MVC
dotnet user-secrets set "EmailSettings:AdminEmail" "<mail admin>" --project MVC
```

Para crear migraciones nuevas: `dotnet ef migrations add <Nombre> --project LogicaAccesoDatos --startup-project MVC`.

## Cosas a tener en cuenta
- Las sesiones están en memoria: cada deploy o reinicio cierra la sesión del admin.
- En PostgreSQL la comparación del usuario en el login distingue mayúsculas y minúsculas (en SQL Server no).
- La contraseña del admin se guarda y compara en texto plano (sin cambios respecto a antes).
- `ResetearRifas` solo inserta las rifas 4001-4080; si ya existen, falla por clave duplicada.
