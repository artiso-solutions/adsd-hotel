FROM mcr.microsoft.com/dotnet/core/runtime:3.1 as base

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build

WORKDIR /
COPY . . 
RUN dotnet publish -c Release -o /app

FROM base as final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "artiso.AdsdHotel.Black.Api.dll"]