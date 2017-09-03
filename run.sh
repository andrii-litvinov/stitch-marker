#!/bin/bash

polymer serve web-app/src/ &
consul agent -dev &
cd backend/src/SM.Service && dotnet restore && dotnet run