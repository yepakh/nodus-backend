#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Nodus.TgBot/Nodus.TgBot.csproj", "Nodus.TgBot/"]
COPY ["Nodus.Database.Context/Nodus.Database.Context.csproj", "Nodus.Database.Context/"]
COPY ["Nodus.Database.Models/Nodus.Database.Models.csproj", "Nodus.Database.Models/"]
RUN dotnet restore "Nodus.TgBot/Nodus.TgBot.csproj"
COPY . .
WORKDIR "/src/Nodus.TgBot"
RUN dotnet build "Nodus.TgBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nodus.TgBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nodus.TgBot.dll"]