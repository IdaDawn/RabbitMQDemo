FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["GJK/GJK.csproj", "GJK/"]
RUN dotnet restore "GJK/GJK.csproj"
COPY . .
WORKDIR "/src/GJK"
RUN dotnet build "GJK.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "GJK.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GJK.dll"]