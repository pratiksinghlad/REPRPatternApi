# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
# Copy csproj and restore dependencies
COPY ["REPRPatternApi.csproj", "."]
RUN dotnet restore "REPRPatternApi.csproj"
# Copy the rest of the code
COPY . .
# Build and publish
RUN dotnet publish "REPRPatternApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create HTTPS directory and certificates in the build stage
RUN mkdir -p /app/.aspnet/https/

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled AS runtime
WORKDIR /app

# Copy the HTTPS directory created in the build stage
COPY --from=build /app/.aspnet/https/ /app/.aspnet/https/

# Environment configuration
ENV ASPNETCORE_URLS=http://+:80;https://+:443 \
    ASPNETCORE_ENVIRONMENT=Local \
    ASPNETCORE_Kestrel__Certificates__Default__Path=/app/.aspnet/https/aspnetapp.pfx \
    ASPNETCORE_Kestrel__Certificates__Default__Password=YourSecurePassword

# Copy published artifacts
COPY --from=build /app/publish .

USER app

ENTRYPOINT ["dotnet", "REPRPatternApi.dll"]