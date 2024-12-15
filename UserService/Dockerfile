FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy SharedLibrary project
COPY ["SharedLibrary/SharedLibrary.csproj", "SharedLibrary/"]

# Copy current service project
COPY ["UserService/UserService.csproj", "UserService/"]

# Restore dependencies, including SharedLibrary
RUN dotnet restore "UserService/UserService.csproj"

# Copy all source files
COPY . .

# Build the service
WORKDIR "/src/UserService"
RUN dotnet build "UserService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.dll"]
