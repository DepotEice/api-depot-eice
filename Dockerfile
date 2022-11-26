#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY "./API.DepotEice.UIL/API.DepotEice.UIL.csproj API.DepotEice.UIL/"
COPY "./API.DepotEice.DAL/API.DepotEice.DAL.csproj API.DepotEice.DAL/"
COPY "./API.DepotEice.Helpers/API.DepotEice.Helpers.csproj API.DepotEice.Helpers/"
RUN dotnet restore "API.DepotEice.UIL/API.DepotEice.UIL.csproj"
COPY . .
WORKDIR "/src/API.DepotEice.UIL"
RUN dotnet build "API.DepotEice.UIL.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.DepotEice.UIL.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.DepotEice.UIL.dll"]