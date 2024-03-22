FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

EXPOSE 8080
    
WORKDIR /src
COPY src/PublisherService/PublisherService.csproj PublisherService/
COPY src/Core/Core.csproj Core/
COPY src/Infrastructure/Infrastructure.csproj Infrastructure/
RUN dotnet restore ./PublisherService/PublisherService.csproj
COPY . .
RUN dotnet build src/PublisherService/PublisherService.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish src/PublisherService/PublisherService.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    APP_DIR="/app/publish" \
    APP_USER=app_user \
    ASPNETCORE_URLS=http://+:8080 
WORKDIR /app
RUN useradd -r -u 1000 ${APP_USER} && chown -R ${APP_USER} . && chmod 700 .
USER ${APP_USER}
COPY --from=publish ${APP_DIR} .
ENTRYPOINT ["dotnet", "PublisherService.dll"]
