FROM microsoft/aspnetcore-build AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers to leverage caching
COPY src/*/*.csproj ./src/
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
