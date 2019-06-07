FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Conductor/Conductor.csproj", "Conductor/"]
COPY ["Conductor.Steps/Conductor.Steps.csproj", "Conductor.Steps/"]
COPY ["Conductor.Domain/Conductor.Domain.csproj", "Conductor.Domain/"]
COPY ["Conductor.Storage/Conductor.Storage.csproj", "Conductor.Storage/"]
RUN dotnet restore "Conductor/Conductor.csproj"
COPY . .
WORKDIR "/src/Conductor"
RUN dotnet build "Conductor.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Conductor.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Conductor.dll"]