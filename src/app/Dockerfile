FROM mcr.microsoft.com/dotnet/core/sdk:3.0 as builder

ADD . /src

WORKDIR /src

RUN dotnet restore

RUN dotnet publish -c Release -o app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0

WORKDIR /app

COPY --from=builder src/app .

ENTRYPOINT ["dotnet", "DogsVsCats.dll"]