FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BinanceCryptoPriceAPI/BinanceCryptoPriceAPI.csproj", "BinanceCryptoPriceAPI/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["Models/Models.csproj", "Models/"]
RUN dotnet restore "./BinanceCryptoPriceAPI/BinanceCryptoPriceAPI.csproj"
COPY . .
WORKDIR "/src/BinanceCryptoPriceAPI"
RUN dotnet build "./BinanceCryptoPriceAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BinanceCryptoPriceAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BinanceCryptoPriceAPI.dll"]