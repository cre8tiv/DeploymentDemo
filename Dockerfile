FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

COPY ["DemoProject/DemoProject.csproj", "./DemoProject/"]
RUN dotnet restore "./DemoProject/"

COPY ["DemoProject.Tests/DemoProject.Tests.csproj", "./DemoProject.Tests/"]
RUN dotnet add "./DemoProject.Tests/DemoProject.Tests.csproj" package dotnet-xunit -v 2.3.0
RUN dotnet restore "./DemoProject.Tests/"

# copy everything else and build
COPY . .
RUN dotnet publish "./DemoProject/" -c Release -o out

# build runtime image
FROM microsoft/aspnetcore:2.0 as web
WORKDIR /app
COPY ["DemoProject/DemoProject.xml", "/app/DemoProject.xml"]
COPY --from=build-env ["/app/DemoProject/out", "."]
CMD ["dotnet", "DemoProject.dll"]