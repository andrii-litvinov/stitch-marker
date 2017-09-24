FROM microsoft/aspnetcore-build AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers to leverage caching
COPY src/SM.Service/*.csproj ./src/SM.Service/
COPY src/SM.Core/*.csproj ./src/SM.Core/
COPY src/SM.Xsd/*.csproj ./src/SM.Xsd/
COPY *.sln ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish src/SM.Service -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore
WORKDIR /app
COPY --from=build-env /app/src/SM.Service/out .
ENTRYPOINT ["dotnet", "SM.Service.dll"]
