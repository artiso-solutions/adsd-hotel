FROM mcr.microsoft.com/dotnet/runtime:5.0 as base

FROM mcr.microsoft.com/dotnet/sdk:5.0 as build

WORKDIR /
COPY . . 
RUN dotnet publish -c Release -o /app

FROM base as final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "artiso.AdsdHotel.Black.Api.dll"]