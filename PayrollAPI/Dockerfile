#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

RUN sed -i'.bak' 's/$/ contrib/' /etc/apt/sources.list
RUN apt-get update; apt-get install -y ttf-mscorefonts-installer fontconfig

WORKDIR /app
COPY logo.jpg /app/
COPY Draft.png /app/
VOLUME /app/images

EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PayrollAPI/PayrollAPI.csproj", "PayrollAPI/"]
RUN dotnet restore "PayrollAPI/PayrollAPI.csproj"
COPY . .
COPY logo.jpg /app/
COPY Draft.png /app/
WORKDIR "/src/PayrollAPI"
COPY logo.jpg /app/
COPY Draft.png /app/
RUN dotnet build "PayrollAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PayrollAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false
COPY logo.jpg /app/
COPY Draft.png /app/

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PayrollAPI.dll"]