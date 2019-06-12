FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["tests/MockServer/MockServer.csproj", "tests/MockServer/"]
RUN dotnet restore "tests/MockServer/MockServer.csproj"
COPY . .
WORKDIR "/src/tests/MockServer"
RUN dotnet build "MockServer.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MockServer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MockServer.dll"]