# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FinansoApp/FinansoApp.csproj", "FinansoApp/"]
COPY ["FinansoData/FinansoData.csproj", "FinansoData/"]
RUN dotnet restore "./FinansoApp/FinansoApp.csproj"
COPY . .
WORKDIR "/src/FinansoApp"
RUN dotnet tool install --global dotnet-ef 
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet build "./FinansoApp.csproj" -c $BUILD_CONFIGURATION -o /app/build
CMD dotnet ef database update --environment Development --project src/project_Repositories --connection "Server=finansoapp-db;Database=YourDatabaseName;User Id=sa;Password=!Password2137*;TrustServerCertificate=True"



# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FinansoApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false




# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN 
ENTRYPOINT ["dotnet", "FinansoApp.dll"]