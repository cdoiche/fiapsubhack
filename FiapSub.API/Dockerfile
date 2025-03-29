FROM mcr.microsoft.com/dotnet/sdk:8.0.300 AS build
WORKDIR /app

COPY FiapSub.sln ./
COPY FiapSub.API/FiapSub.API.csproj FiapSub.API/
COPY FiapSub.Core/FiapSub.Core.csproj FiapSub.Core/
COPY FiapSub.Infra/FiapSub.Infra.csproj FiapSub.Infra/
COPY FiapSub.Tests/FiapSub.Tests.csproj FiapSub.Tests/

RUN dotnet restore

COPY . ./

RUN dotnet publish FiapSub.API/FiapSub.API.csproj -c Release -o /publish

FROM mcr.microsoft.com/dotnet/sdk:8.0.300 AS runtime
WORKDIR /app

ENV PATH="/root/.dotnet/tools:$PATH"
RUN dotnet tool install --global dotnet-ef

COPY --from=build /publish .

EXPOSE 5000

ENTRYPOINT ["dotnet", "FiapSub.API.dll"]