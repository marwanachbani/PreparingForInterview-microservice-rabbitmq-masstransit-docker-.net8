FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy SharedLibrary project
COPY ["SharedLibrary/SharedLibrary.csproj", "SharedLibrary/"]

# Copy EvenStoreService project
COPY ["EvenStoreService/EvenStoreService.csproj", "EvenStoreService/"]

# Restore dependencies
RUN dotnet restore "EvenStoreService/EvenStoreService.csproj"

# Copy all source files
COPY ./SharedLibrary ./SharedLibrary
COPY ./EvenStoreService ./EvenStoreService

# Build the service
WORKDIR "/src/EvenStoreService"
RUN dotnet build "EvenStoreService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EvenStoreService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EvenStoreService.dll"]
