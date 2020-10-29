#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Microservice.Gateway/Microservice.Gateway.csproj", "Microservice.Gateway/"]
RUN dotnet restore "Microservice.Gateway/Microservice.Gateway.csproj"
COPY . .
WORKDIR "/src/Microservice.Gateway"
RUN dotnet build "Microservice.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Microservice.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Microservice.Gateway.dll"]