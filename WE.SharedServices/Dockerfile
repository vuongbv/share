
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WE.SharedServices/src/WE.Shared.Service/WE.Shared.Service.csproj", "WE.SharedServices/src/WE.Shared.Service/"]
COPY ["WE.SharedServices/src/WE.Shared.Service.Application/WE.Shared.Service.Application.csproj", "WE.SharedServices/src/WE.Shared.Service.Application/"]
COPY ["WE.SharedServices/src/WE.Shared.Service.Business.Store/WE.Shared.Service.Domain.csproj", "WE.SharedServices/src/WE.Shared.Service.Business.Store/"]
COPY ["WE.Common/src/Common.csproj", "WE.Common/src/"]

RUN dotnet restore "WE.SharedServices/src/WE.Shared.Service/WE.Shared.Service.csproj"
COPY . .
WORKDIR "/src/WE.SharedServices/src/WE.Shared.Service"
RUN dotnet build "WE.Shared.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WE.Shared.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WE.Shared.Service.dll"]