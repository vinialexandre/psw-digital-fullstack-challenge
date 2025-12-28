FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/HolidaysAPI.API/HolidaysAPI.API.csproj", "HolidaysAPI.API/"]
COPY ["src/HolidaysAPI.Application/HolidaysAPI.Application.csproj", "HolidaysAPI.Application/"]
COPY ["src/HolidaysAPI.Domain/HolidaysAPI.Domain.csproj", "HolidaysAPI.Domain/"]
COPY ["src/HolidaysAPI.Infrastructure/HolidaysAPI.Infrastructure.csproj", "HolidaysAPI.Infrastructure/"]

RUN dotnet restore "HolidaysAPI.API/HolidaysAPI.API.csproj"

COPY src/ .

WORKDIR "/src/HolidaysAPI.API"
RUN dotnet build "HolidaysAPI.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HolidaysAPI.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HolidaysAPI.API.dll"]

