FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /VIRCE_server

COPY . .
RUN dotnet restore ./VIRCE_server
RUN dotnet publish -c Release -o out ./VIRCE_server

FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /VIRCE_server
COPY --from=build-env /VIRCE_server/out .
ENTRYPOINT ["dotnet", "VIRCE_server.dll"]