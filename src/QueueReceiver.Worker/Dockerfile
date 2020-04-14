FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/QueueReceiver.Worker/QueueReceiver.Worker.csproj", "src/QueueReceiver.Worker/"]
RUN dotnet restore "src/QueueReceiver.Worker/QueueReceiver.Worker.csproj"
COPY . .
WORKDIR "/src/src/QueueReceiver.Worker"
RUN dotnet build "QueueReceiver.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QueueReceiver.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QueueReceiver.Worker.dll"]