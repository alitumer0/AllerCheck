FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AllerCheck.UI/AllerCheck.UI.csproj", "AllerCheck.UI/"]
COPY ["AllerCheck_Core/AllerCheck_Core.csproj", "AllerCheck_Core/"]
COPY ["AllerCheck_Data/AllerCheck_Data.csproj", "AllerCheck_Data/"]
COPY ["AllerCheck_Services/AllerCheck_Services.csproj", "AllerCheck_Services/"]
COPY ["AllerCheck_Mapping/AllerCheck_Mapping.csproj", "AllerCheck_Mapping/"]
RUN dotnet restore "AllerCheck.UI/AllerCheck.UI.csproj"
COPY . .
WORKDIR "/src/AllerCheck.UI"
RUN dotnet build "AllerCheck.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AllerCheck.UI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AllerCheck.UI.dll"] 