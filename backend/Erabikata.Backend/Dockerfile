FROM registry.apps.lasath.org/erabikata-data
WORKDIR /app
EXPOSE 5000

COPY bin/Release/netcoreapp3.1/publish .
ENTRYPOINT ["dotnet", "Erabikata.Backend.dll"]
