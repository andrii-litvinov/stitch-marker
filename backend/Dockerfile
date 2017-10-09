FROM microsoft/aspnetcore-build AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers to cache
COPY src/SM.Service/*.csproj ./
RUN dotnet restore -r linux-x64

# Copy everything else and build
COPY src/SM.Service ./
RUN dotnet publish -r linux-x64 -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2-runtime-jessie

# Install libfontconfig1 dependency for SkiaSharp and cache
RUN apt-get update && apt-get install -y libfontconfig1

WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["./SM.Service"]
