FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["codewave-roots-licence-server/codewave-roots-licence-server.csproj", "codewave-roots-licence-server/"]
RUN dotnet restore "codewave-roots-licence-server/codewave-roots-licence-server.csproj"
COPY . .
WORKDIR "/src/codewave-roots-licence-server"
RUN dotnet build "codewave-roots-licence-server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "codewave-roots-licence-server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "codewave-roots-licence-server.dll"]
