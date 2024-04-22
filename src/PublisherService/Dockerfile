FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release    
WORKDIR /src
COPY src/PublisherService/PublisherService.csproj PublisherService/
COPY src/Core/Core.csproj Core/
COPY src/Infrastructure/Infrastructure.csproj Infrastructure/
RUN dotnet restore ./PublisherService/PublisherService.csproj
COPY . .
RUN dotnet build src/PublisherService/PublisherService.csproj -c BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish src/PublisherService/PublisherService.csproj -c BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine as final
EXPOSE 8080
ARG APP_DIR="app/publish"
ARG APP_USER=app_user
ARG ASPNETCORE_URLS=http://+:8080
WORKDIR /app
RUN adduser -D ${APP_USER} && chown -R ${APP_USER} . && chmod 700 .
USER ${APP_USER}
COPY --from=publish ${APP_DIR} .
ENTRYPOINT ["dotnet", "PublisherService.dll"]
