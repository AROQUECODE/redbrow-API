#Start with the base .NET SDK Image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS build-env

#Set the working directory inside the container
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["aroque.code.API/aroque.code.API.csproj", "aroque.code.API/"]
COPY ["aroque.code.response/aroque.code.response.csproj", "aroque.code.response/"]
RUN dotnet restore "aroque.code.API/aroque.code.API.csproj"
COPY . .
WORKDIR "/src/aroque.code.API"
RUN dotnet build "aroque.code.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "aroque.code.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "aroque.code.API.dll"]