FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal
WORKDIR /app
EXPOSE 5000

RUN ["apt-get", "--assume-yes", "update"]
RUN ["apt-get", "--assume-yes", "install", "ffmpeg"]

COPY bin/Release/net6.0/publish .
ENTRYPOINT ["dotnet", "Erabikata.Backend.dll"]
