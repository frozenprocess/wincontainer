FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY *.csproj . 
RUN dotnet restore -r win-x64 /p:PublishReadyToRun=true

COPY . .
RUN dotnet publish -c release -o /app -r win-x64 --self-contained true --no-restore /p:PublishTrimmed=true

FROM mcr.microsoft.com/windows/nanoserver:1809
WORKDIR /app
COPY --from=build /app .

EXPOSE 80

ENTRYPOINT ["wincontainer.exe","--urls","http://0.0.0.0"]