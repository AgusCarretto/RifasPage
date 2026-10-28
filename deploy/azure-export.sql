-- Exporta Admin, Compradores y Rifas de Azure SQL como sentencias INSERT para PostgreSQL.
--
-- Cómo usarlo:
--   1. Ejecutá este script en Azure SQL (Query editor del portal, Azure Data Studio o SSMS).
--   2. Devuelve UNA sola celda con todo el SQL. Copiá su contenido y guardalo como import.sql.
--      (En SSMS: "Results to Text" o copiar la celda; en Azure Data Studio: clic derecho > Copy.)
--   3. Primero aplicá las migraciones en Postgres (la app lo hace sola al arrancar con
--      Database__MigrateOnStartup=true) y después ejecutá import.sql con psql.
--      Detalle en docs/DEPLOY-RAILWAY.md.
--
-- Las tablas tienen que estar vacías en Postgres antes de importar (si la app ya creó filas, el import falla
-- por claves duplicadas y no deja nada a medias porque va dentro de una transacción).

SET NOCOUNT ON;

SELECT
    'BEGIN;' + CHAR(10)

    + ISNULL((
        SELECT STRING_AGG(CONVERT(nvarchar(max),
            'INSERT INTO "Admin" ("id","name","password","email") VALUES ('
            + CAST(a.id AS nvarchar(20)) + ','''
            + REPLACE(a.name, '''', '''''') + ''','''
            + REPLACE(a.password, '''', '''''') + ''','''
            + REPLACE(a.email, '''', '''''') + ''');'
        ), CHAR(10)) WITHIN GROUP (ORDER BY a.id)
        FROM dbo.Admin a
    ), '') + CHAR(10)

    + ISNULL((
        SELECT STRING_AGG(CONVERT(nvarchar(max),
            'INSERT INTO "Compradores" ("id","name","phoneNumber","email") VALUES ('
            + CAST(c.id AS nvarchar(20)) + ','''
            + REPLACE(c.name, '''', '''''') + ''','''
            + REPLACE(c.phoneNumber, '''', '''''') + ''','''
            + REPLACE(c.email, '''', '''''') + ''');'
        ), CHAR(10)) WITHIN GROUP (ORDER BY c.id)
        FROM dbo.Compradores c
    ), '') + CHAR(10)

    + ISNULL((
        SELECT STRING_AGG(CONVERT(nvarchar(max),
            'INSERT INTO "Rifas" ("id","prize","CompradorId","state") VALUES ('
            + CAST(r.id AS nvarchar(20)) + ','
            + CAST(r.prize AS nvarchar(40)) + ','
            + ISNULL(CAST(r.CompradorId AS nvarchar(20)), 'NULL') + ','
            + CAST(r.state AS nvarchar(20)) + ');'
        ), CHAR(10)) WITHIN GROUP (ORDER BY r.id)
        FROM dbo.Rifas r
    ), '') + CHAR(10)

    -- Admin y Compradores usan identity: tras insertar ids explícitos hay que avanzar la secuencia.
    + 'SELECT setval(pg_get_serial_sequence(''"Admin"'', ''id''), COALESCE(MAX("id"), 0) + 1, false) FROM "Admin";' + CHAR(10)
    + 'SELECT setval(pg_get_serial_sequence(''"Compradores"'', ''id''), COALESCE(MAX("id"), 0) + 1, false) FROM "Compradores";' + CHAR(10)
    + 'COMMIT;'
    AS postgres_import_sql;
