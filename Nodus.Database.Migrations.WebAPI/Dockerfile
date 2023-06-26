#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Nodus.Database.Migrations.WebAPI/Nodus.Database.Migrations.WebAPI.csproj", "Nodus.Database.Migrations.WebAPI/"]
COPY ["Nodus.Database.Migrator/Nodus.Database.Migrator.csproj", "Nodus.Database.Migrator/"]
COPY ["Nodus.Database.Context/Nodus.Database.Context.csproj", "Nodus.Database.Context/"]
COPY ["Nodus.Database.Models/Nodus.Database.Models.csproj", "Nodus.Database.Models/"]
RUN dotnet restore "Nodus.Database.Migrations.WebAPI/Nodus.Database.Migrations.WebAPI.csproj"
COPY . .
WORKDIR "/src/Nodus.Database.Migrations.WebAPI"
RUN dotnet build "Nodus.Database.Migrations.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nodus.Database.Migrations.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nodus.Database.Migrations.WebAPI.dll"]