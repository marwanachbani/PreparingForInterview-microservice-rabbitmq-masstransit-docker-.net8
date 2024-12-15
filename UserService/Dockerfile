# Use .NET 8 SDK as the base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files and restore dependencies
# Copy SharedLibrary project
COPY ["SharedLibrary/SharedLibrary.csproj", "SharedLibrary/"]
COPY *.csproj ./
RUN dotnet restore

# Copy remaining files and build the project
COPY . ./
RUN dotnet publish -c Release -o out

# Use ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "UserService.dll"]
