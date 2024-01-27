#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Gateway/Gateway.csproj", "Gateway/"]
COPY ["AccountsManager/AccountsManager.csproj", "AccountsManager/"]
COPY ["Authentication/Authentication.csproj", "Authentication/"]
COPY ["Models/Models.csproj", "Models/"]
COPY ["DataManager/DataManager.csproj", "DataManager/"]
COPY ["DeveloperPortal/DeveloperPortal.csproj", "DeveloperPortal/"]
COPY ["GameManager/GameManager.csproj", "GameManager/"]
COPY ["LobbyManager/LobbyManager.csproj", "LobbyManager/"]
COPY ["PaymentManager/PaymentManager.csproj", "PaymentManager/"]
RUN dotnet restore "Gateway/Gateway.csproj"
COPY . .
WORKDIR "/src/Gateway"
RUN dotnet build "Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.dll"]