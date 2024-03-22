FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    APP_DIR="/app/publish" \
    APP_USER=app_user \
    
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/MessageProcessor/MessageProcessor.csproj", "MessageProcessor/"]
COPY ["src/Core/Core.csproj", "Core/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "MessageProcessor/MessageProcessor.csproj"
COPY . .
RUN dotnet build "src/MessageProcessor/MessageProcessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/MessageProcessor/MessageProcessor.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
RUN useradd -r -u 1000 ${APP_USER} && chown ${APP_USER} .
USER ${APP_USER}
COPY --from=publish ${APP_DIR} .
ENTRYPOINT ["dotnet", "MessageProcessor.dll"]