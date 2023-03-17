FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80/tcp

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["secondapp.csproj", "./"]
RUN dotnet restore "secondapp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "secondapp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "secondapp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "secondapp.dll"]