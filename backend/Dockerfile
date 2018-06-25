FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/SM.Service/*.csproj ./
RUN dotnet restore -r linux-x64

# Copy everything else and build
COPY src/SM.Service ./
RUN dotnet publish -r linux-x64 -c Release -o out



FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime

# Install libfontconfig1 dependency for SkiaSharp
RUN apt-get update && apt-get install -y libfontconfig1

WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "SM.Service.dll"]
