# Multi-stage build for zako-issuetracker (Discord bot)
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore (speeds up rebuilds when sources change)
COPY ["zako-issuetracker.csproj", "./"]
RUN dotnet restore "zako-issuetracker.csproj"

# Copy everything and publish
COPY . .
RUN dotnet publish "zako-issuetracker.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app

# Default sqlite file location inside container (can be overridden by env)
ENV SQLITE_FILE=/app/data/zako.db

# Create a place for the sqlite file when used as a volume
RUN mkdir -p /app/data

# Copy published output
COPY --from=build /app/publish .

# The bot expects DISCORD_TOKEN and ADMIN_IDS environment variables (and can use SQLITE_FILE)
# Example: docker run -e DISCORD_TOKEN=xxx -e ADMIN_IDS="123,456" -v ~/zako-data:/app/data <image>

ENTRYPOINT ["dotnet", "zako-issuetracker.dll"]
