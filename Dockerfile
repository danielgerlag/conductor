FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
ENV DBHOST mongodb://10.225.2.96:27017/
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY src .
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