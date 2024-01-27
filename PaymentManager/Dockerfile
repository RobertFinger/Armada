#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PaymentManager/PaymentManager.csproj", "PaymentManager/"]
RUN dotnet restore "PaymentManager/PaymentManager.csproj"
COPY . .
WORKDIR "/src/PaymentManager"
RUN dotnet build "PaymentManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentManager.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentManager.dll"]