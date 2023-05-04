FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Corgibytes.Freshli.Agent.DotNet/Corgibytes.Freshli.Agent.DotNet.csproj", "Corgibytes.Freshli.Agent.DotNet/"]
RUN dotnet restore "Corgibytes.Freshli.Agent.DotNet/Corgibytes.Freshli.Agent.DotNet.csproj"
COPY . .
WORKDIR "/src/Corgibytes.Freshli.Agent.DotNet"
RUN dotnet build "Corgibytes.Freshli.Agent.DotNet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Corgibytes.Freshli.Agent.DotNet.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Corgibytes.Freshli.Agent.DotNet.dll"]
