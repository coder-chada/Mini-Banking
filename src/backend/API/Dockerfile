FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the .csproj files first
COPY *.sln .
COPY DomainLogic/DomainLogic.csproj   DomainLogic/
COPY ApplicationService/ApplicationService.csproj   ApplicationService/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY API/API.csproj API/

# Docker reuses the cached restore layer if projects references or NuGet package doesn't changes
RUN dotnet restore API/API.csproj

# Now copy everything else (source code)
COPY . .

# Publish (restore is skipped since nothing relevant changed, if csproj files didn't change)
RUN dotnet publish API/API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Generate the publish image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "API.dll"]