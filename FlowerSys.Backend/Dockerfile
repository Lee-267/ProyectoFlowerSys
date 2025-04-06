# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar el archivo de proyecto y restaurar dependencias
COPY FlowerSys.Backend.csproj .
RUN dotnet restore

# Copiar el resto del código y compilar
COPY . .
RUN dotnet publish -c Release -o /out

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Puerto por defecto para ASP.NET Core
EXPOSE 80

# Comando de inicio
ENTRYPOINT ["dotnet", "FlowerSys.Backend.dll"]
