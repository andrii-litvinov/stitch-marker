FROM microsoft/aspnetcore-build AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers to leverage caching
COPY src/SM.Service/*.csproj ./src/SM.Service/
COPY src/SM.Core/*.csproj ./src/SM.Core/
COPY src/SM.Xsd/*.csproj ./src/SM.Xsd/
COPY *.sln ./
RUN dotnet restore -r linux-x64

# Copy everything else and build
COPY . ./
RUN dotnet publish src/SM.Service -r linux-x64 -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2-runtime-jessie
RUN apt-get update && apt-get install -y libfontconfig1
WORKDIR /app
COPY --from=build-env /app/src/SM.Service/out .
ENTRYPOINT ["./SM.Service"]
