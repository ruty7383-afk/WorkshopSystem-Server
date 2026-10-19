FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore "WorkshopSystem.API/WorkshopSystem.API.csproj"

RUN dotnet publish "WorkshopSystem.API/WorkshopSystem.API.csproj" `
    -c Release `
    -o /app/publish `
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "WorkshopSystem.API.dll"]
