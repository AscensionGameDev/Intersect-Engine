FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Intersect.OpenPortChecker/Intersect.OpenPortChecker.csproj", "Intersect.OpenPortChecker/"]
RUN dotnet restore "Intersect.OpenPortChecker/Intersect.OpenPortChecker.csproj"
COPY . .
WORKDIR "/src/Intersect.OpenPortChecker"
RUN dotnet build "Intersect.OpenPortChecker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Intersect.OpenPortChecker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Intersect.OpenPortChecker.dll"]
