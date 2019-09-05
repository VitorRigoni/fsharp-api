FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
COPY ["src/FSharpApi/FSharpApi.fsproj", "src/FSharpApi/FSharpApi.fsproj"]
COPY . .
RUN dotnet build "src/FSharpApi/FSharpApi.fsproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "src/FSharpApi/FSharpApi.fsproj" -c Release -o /app --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FSharpApi.dll"]
