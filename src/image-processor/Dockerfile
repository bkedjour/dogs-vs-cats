FROM mcr.microsoft.com/dotnet/core/sdk:3.0 as builder

ADD . /src

WORKDIR /src

RUN dotnet restore

RUN dotnet publish -c Release -r debian-x64 -o app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0

RUN apt-get update
RUN apt-get install libfontconfig1 -y

WORKDIR /app

COPY --from=builder src/app .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ImageProcessor.dll"]