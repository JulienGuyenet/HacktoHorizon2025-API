# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Install CA certificates to fix SSL issues
RUN apt-get update && apt-get install -y ca-certificates && update-ca-certificates

# Copy solution and project files
COPY FurnitureInventory.sln .
COPY src/FurnitureInventory.Core/FurnitureInventory.Core.csproj src/FurnitureInventory.Core/
COPY src/FurnitureInventory.Infrastructure/FurnitureInventory.Infrastructure.csproj src/FurnitureInventory.Infrastructure/
COPY src/FurnitureInventory.Api/FurnitureInventory.Api.csproj src/FurnitureInventory.Api/

# Restore dependencies
RUN dotnet restore

# Copy all source files
COPY src/ src/

# Build and publish the application
WORKDIR /source/src/FurnitureInventory.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .
# Copy xlsx data files
COPY /src/data /app/data

# Create directory for SQLite database
RUN mkdir -p /app/data

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/furnitureinventory.db"

# Expose port
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "FurnitureInventory.Api.dll"]
