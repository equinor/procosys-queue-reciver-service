FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/QueueReciver.Worker/QueueReciver.Worker.csproj", "src/QueueReciver.Worker/"]
RUN dotnet restore "src/QueueReciver.Worker/QueueReciver.Worker.csproj"
COPY . .
WORKDIR "/src/src/src/QueueReciver.Worker"
RUN dotnet build "QueueReciver.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QueueReciver.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QueueReciver.Worker.dll"]