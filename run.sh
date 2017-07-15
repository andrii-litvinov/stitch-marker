#!/bin/bash

polymer serve web-app/src/ &
cd backend/src/SM.Service && dotnet restore && dotnet run