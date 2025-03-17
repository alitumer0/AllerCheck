FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only necessary projects
COPY ["AllerCheck.UI/AllerCheck.UI.csproj", "AllerCheck.UI/"]
COPY ["AllerCheck_Core/AllerCheck_Core.csproj", "AllerCheck_Core/"]
COPY ["AllerCheck_Data/AllerCheck_Data.csproj", "AllerCheck_Data/"]
COPY ["AllerCheck_Services/AllerCheck_Services.csproj", "AllerCheck_Services/"]
COPY ["AllerCheck_Mapping/AllerCheck_Mapping.csproj", "AllerCheck_Mapping/"]
COPY ["AllerCheck_DTO/AllerCheck_DTO.csproj", "AllerCheck_DTO/"]

# Restore dependencies
RUN dotnet restore "AllerCheck.UI/AllerCheck.UI.csproj"

# Copy only UI and related projects
COPY AllerCheck.UI/. AllerCheck.UI/
COPY AllerCheck_Core/. AllerCheck_Core/
COPY AllerCheck_Data/. AllerCheck_Data/
COPY AllerCheck_Services/. AllerCheck_Services/
COPY AllerCheck_Mapping/. AllerCheck_Mapping/
COPY AllerCheck_DTO/. AllerCheck_DTO/

# Build and publish
WORKDIR "/src/AllerCheck.UI"
RUN dotnet publish "AllerCheck.UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AllerCheck.UI.dll"] 