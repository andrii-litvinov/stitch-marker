FROM microsoft/aspnetcore-build AS build-env
WORKDIR /app

# Copy everything and build
COPY . ./
RUN dotnet publish src/SM.Service -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore
WORKDIR /app
COPY --from=build-env /app/src/SM.Service/out .
ENTRYPOINT ["dotnet", "SM.Service.dll"]
