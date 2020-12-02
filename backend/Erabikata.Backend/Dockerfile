FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
EXPOSE 5000

COPY bin/Release/netcoreapp3.1/publish .
ENTRYPOINT ["dotnet", "Erabikata.Backend.dll"]
