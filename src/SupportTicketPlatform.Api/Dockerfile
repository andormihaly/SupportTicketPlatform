# syntax=docker/dockerfile:1.19

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Cache restore until project dependencies or shared build settings change.
COPY ["Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["src/SupportTicketPlatform.Api/SupportTicketPlatform.Api.csproj", "src/SupportTicketPlatform.Api/"]
COPY ["src/SupportTicketPlatform.Application/SupportTicketPlatform.Application.csproj", "src/SupportTicketPlatform.Application/"]
COPY ["src/SupportTicketPlatform.Contracts/SupportTicketPlatform.Contracts.csproj", "src/SupportTicketPlatform.Contracts/"]
COPY ["src/SupportTicketPlatform.Domain/SupportTicketPlatform.Domain.csproj", "src/SupportTicketPlatform.Domain/"]
COPY ["src/SupportTicketPlatform.Infrastructure/SupportTicketPlatform.Infrastructure.csproj", "src/SupportTicketPlatform.Infrastructure/"]
COPY ["src/SupportTicketPlatform.Persistence/SupportTicketPlatform.Persistence.csproj", "src/SupportTicketPlatform.Persistence/"]
COPY ["src/SupportTicketPlatform.ServiceDefaults/SupportTicketPlatform.ServiceDefaults.csproj", "src/SupportTicketPlatform.ServiceDefaults/"]
RUN dotnet restore "src/SupportTicketPlatform.Api/SupportTicketPlatform.Api.csproj"


COPY src/ ./src/
RUN dotnet publish "src/SupportTicketPlatform.Api/SupportTicketPlatform.Api.csproj" \
    --configuration Release --no-restore --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "SupportTicketPlatform.Api.dll"]
