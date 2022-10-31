#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["st-dotnet/st-dotnet.csproj", "st-dotnet/"]
RUN dotnet restore "st-dotnet/st-dotnet.csproj"
COPY . .
WORKDIR "/src/st-dotnet"
RUN dotnet build "st-dotnet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "st-dotnet.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "st-dotnet.dll"]
