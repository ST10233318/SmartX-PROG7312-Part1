# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy the project file
COPY *.csproj ./

# Restore NuGet packages
RUN dotnet restore

# Copy the remaining source code
COPY . .

# Build the application
RUN dotnet build -c Release -o /app/build


# Stage 2: Publish the application
FROM build AS publish

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false


# Stage 3: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080

# Copy the published application
COPY --from=publish /app/publish .

# Expose the HTTP port
EXPOSE 8080

# Start the Smart-X API
ENTRYPOINT ["dotnet", "SmartX.Api.dll"]