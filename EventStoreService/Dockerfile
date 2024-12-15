FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy SharedLibrary project
COPY ["SharedLibrary/SharedLibrary.csproj", "SharedLibrary/"]

# Copy service-specific project
COPY ["EventStoreService/EventStoreService.csproj", "EventStoreService/"]

# Restore dependencies, including SharedLibrary
RUN dotnet restore "EventStoreService/EventStoreService.csproj"

# Copy all source files
COPY ./SharedLibrary ./SharedLibrary
COPY ./EventStoreService ./EventStoreService

# Build the service
WORKDIR "/src/EventStoreService"
RUN dotnet build "EventStoreService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EventStoreService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventStoreService.dll"]
