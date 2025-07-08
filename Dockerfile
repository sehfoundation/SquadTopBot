# Базовий імідж для запуску додатку
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Базовий імідж для збирання додатку
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ./SharpBotTopOnline.csproj
RUN dotnet publish ./SharpBotTopOnline.csproj -c Release -o /app/publish

# Збираємо фінальний імідж
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SharpBotTopOnline.dll"]
