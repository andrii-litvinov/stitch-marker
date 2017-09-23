FROM microsoft/dotnet:sdk

ENV DOTNET_USE_POLLING_FILE_WATCHER=1
ENV ASPNETCORE_URLS=http://0.0.0.0:5000

WORKDIR /dotnetapp/SM.Service

CMD ["/bin/bash", "-c", "dotnet run"]
