# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
# Paths are now relative to repo root
COPY ["backend/src/Backend.Api/Backend.Api.csproj", "backend/src/Backend.Api/"]
RUN dotnet restore "backend/src/Backend.Api/Backend.Api.csproj"

# Copy everything else and build
COPY backend/ backend/
WORKDIR "/src/backend/src/Backend.Api"
RUN dotnet publish "Backend.Api.csproj" -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Install curl for healthchecks if needed, but minimal is fine
# Expose port (Render sets PORT env var, ASP.NET Core 8 listens on 8080 by default)
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "Backend.Api.dll"]
