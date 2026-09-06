FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GameStore.UI/GameStore.UI.csproj", "GameStore.UI/"]
COPY ["GameStore.Application/GameStore.Application.csproj", "GameStore.Application/"]
COPY ["GameStore.Domain/GameStore.Domain.csproj", "GameStore.Domain/"]
COPY ["GameStore.Infrastructure/GameStore.Infrastructure.csproj", "GameStore.Infrastructure/"]
RUN dotnet restore "GameStore.UI/GameStore.UI.csproj"
COPY . .
WORKDIR "/src/GameStore.UI"
RUN dotnet build "./GameStore.UI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GameStore.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameStore.UI.dll"]
