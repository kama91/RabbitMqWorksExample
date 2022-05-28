FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/PublisherService/PublisherService.csproj", "PublisherService/"]
COPY ["src/Core/Core.csproj", "Core/"]
RUN dotnet restore "PublisherService/PublisherService.csproj"
COPY . .
WORKDIR "/src/src/PublisherService"
RUN dotnet build "PublisherService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PublisherService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PublisherService.dll"]