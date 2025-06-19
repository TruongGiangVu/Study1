# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy everything and restore
COPY . .
RUN dotnet restore

# Build and publish
RUN dotnet publish -c Release -o out

# Use the ASP.NET runtime image for hosting
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/out .

# Expose the default ASP.NET Core port
EXPOSE 80

# Start the app
ENTRYPOINT ["dotnet", "GenApi.dll"]
