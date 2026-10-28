# Imagen para desplegar en Railway (Railpack todavía no soporta .NET, por eso se usa Dockerfile).
# El contexto de build es la raíz del repo, así que se copian las tres carpetas de proyecto.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore primero: esta capa se cachea mientras no cambien los .csproj.
COPY ["Biblioteca de clases/Biblioteca de clases/LogicaNegocio.csproj", "Biblioteca de clases/Biblioteca de clases/"]
COPY ["LogicaAccesoDatos/LogicaAccesoDatos.csproj", "LogicaAccesoDatos/"]
COPY ["MVC/MVC.csproj", "MVC/"]
RUN dotnet restore "MVC/MVC.csproj"

COPY ["Biblioteca de clases/", "Biblioteca de clases/"]
COPY ["LogicaAccesoDatos/", "LogicaAccesoDatos/"]
COPY ["MVC/", "MVC/"]
RUN dotnet publish "MVC/MVC.csproj" -c Release -o /app/publish --no-restore -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# - FORWARDEDHEADERS: Railway termina el TLS y reenvía por HTTP; así la app ve el esquema/IP originales.
# - MigrateOnStartup: aplica las migraciones de EF al arrancar (ver Program.cs).
# El puerto lo toma la app de la variable PORT que inyecta Railway.
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_FORWARDEDHEADERS_ENABLED=true \
    Database__MigrateOnStartup=true

USER $APP_UID
ENTRYPOINT ["dotnet", "MVC.dll"]
