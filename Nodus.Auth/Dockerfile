#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Nodus.Auth/Nodus.Auth.csproj", "Nodus.Auth/"]
COPY ["Nodus.Auth.Handler/Nodus.Auth.Handler.csproj", "Nodus.Auth.Handler/"]
COPY ["Nodus.Database.Context/Nodus.Database.Context.csproj", "Nodus.Database.Context/"]
COPY ["Nodus.Database.Models/Nodus.Database.Models.csproj", "Nodus.Database.Models/"]
RUN dotnet restore "Nodus.Auth/Nodus.Auth.csproj"
COPY . .
WORKDIR "/src/Nodus.Auth"
RUN dotnet build "Nodus.Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nodus.Auth.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nodus.Auth.dll"]