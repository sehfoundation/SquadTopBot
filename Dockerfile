FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

ENV TOKEN_BOT=${TOKEN_BOT}
ENV TOKEN_BM=${TOKEN_BM}
ENV SERVER_ID_SQ_1=${SERVER_ID_SQ_1}
ENV SERVER_ID_SQ_2=${SERVER_ID_SQ_2}

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ./SharpBotTopOnline.csproj
RUN dotnet publish ./SharpBotTopOnline.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SharpBotTopOnline.dll"]
