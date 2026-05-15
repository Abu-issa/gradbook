FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["GradBook.Web/GradBook.Web.csproj", "GradBook.Web/"]
COPY ["GradBook.Application/GradBook.Application.csproj", "GradBook.Application/"]
COPY ["GradBook.Domain/GradBook.Domain.csproj", "GradBook.Domain/"]
COPY ["GradBook.Infrastructure/GradBook.Infrastructure.csproj", "GradBook.Infrastructure/"]

RUN dotnet restore "GradBook.Web/GradBook.Web.csproj"

COPY . .
WORKDIR "/src/GradBook.Web"
RUN dotnet build "GradBook.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GradBook.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create uploads directory
RUN mkdir -p wwwroot/uploads

ENTRYPOINT ["dotnet", "GradBook.Web.dll"]
