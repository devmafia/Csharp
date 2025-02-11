# Use the ASP.NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
EXPOSE 5002

# Use the SDK image for building and publishing the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore

# Keep the application in Debug mode for hot reload
RUN dotnet build -c Debug -o /app/build

# Publish the application (for production builds)
FROM build AS publish
RUN dotnet publish -c Debug -o /app/publish

# Use the ASP.NET Core runtime for running the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /src
COPY --from=publish /app/publish .
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
ENV ASPNETCORE_ENVIRONMENT=Development

# Run the application in watch mode
CMD ["dotnet", "watch", "run", "--no-launch-profile", "--urls=http://0.0.0.0:5002"]
