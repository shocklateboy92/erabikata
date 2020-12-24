FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-focal
WORKDIR /app
EXPOSE 5000

RUN ["apt-get", "--assume-yes", "update"]
RUN ["apt-get", "--assume-yes", "install", "ffmpeg"]

COPY bin/Release/netcoreapp3.1/publish .
ENTRYPOINT ["dotnet", "Erabikata.Backend.dll"]
