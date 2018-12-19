FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["mosPortal/mosPortal.csproj", "mosPortal/"]
RUN dotnet restore "mosPortal/mosPortal.csproj"
COPY . .
WORKDIR "/src/mosPortal"
RUN dotnet build "mosPortal.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "mosPortal.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "mosPortal.dll"]