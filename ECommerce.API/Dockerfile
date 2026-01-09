# Build / Publish
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything (solution + projects)
COPY . .

# Restore + publish the API project
RUN dotnet restore ./ECommerce.API/ECommerce.API.csproj
RUN dotnet publish ./ECommerce.API/ECommerce.API.csproj -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Serilog writes files to Logs/
RUN mkdir -p /app/Logs

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ECommerce.API.dll"]
