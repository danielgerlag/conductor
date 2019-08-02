FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
ENV DBHOST mongodb://10.225.2.96:27017/
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
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