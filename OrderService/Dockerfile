FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy SharedLibrary project
COPY ["SharedLibrary/SharedLibrary.csproj", "SharedLibrary/"]

# Copy OrderService project
COPY ["OrderService/OrderService.csproj", "OrderService/"]

# Restore dependencies
RUN dotnet restore "OrderService/OrderService.csproj"

# Copy all source files
COPY ./SharedLibrary ./SharedLibrary
COPY ./OrderService ./OrderService

# Build the service
WORKDIR "/src/OrderService"
RUN dotnet build "OrderService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.dll"]
