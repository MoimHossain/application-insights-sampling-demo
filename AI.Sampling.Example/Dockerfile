FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["AI.Sampling.Example.csproj", ""]
RUN dotnet restore "./AI.Sampling.Example.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "AI.Sampling.Example.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AI.Sampling.Example.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AI.Sampling.Example.dll"]