# Use .NET SDK for building the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the .csproj file and restore dependencies
# Copy SharedLibrary project
COPY ["SharedLibrary/SharedLibrary.csproj", "SharedLibrary/"]
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the files and build the app
COPY . ./
RUN dotnet publish -c Release -o out

# Use ASP.NET Core runtime for the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Expose the port your application will run on


# Set the entry point for the container to the ProductService.dll
ENTRYPOINT ["dotnet", "OrderService.dll"]
