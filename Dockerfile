FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base

EXPOSE 7247
EXPOSE 5013

WORKDIR /app

ENV ASPNETCORE_URLS=https://+:7247
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Crosscutting/Crosscutting.csproj", "Crosscutting/"]
COPY ["src/Data/Data.csproj", "Data/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/SharedKernel/SharedKernel.csproj", "SharedKernel/"]
COPY ["src/WebApi/WebApi.csproj", "WebApi/"]

RUN dotnet restore "WebApi/WebApi.csproj"
COPY . .

WORKDIR "/src/WebApi"
RUN dotnet build "WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]
