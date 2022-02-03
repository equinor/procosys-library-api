FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy solution and project files separately and restore NuGet packages to create layers
COPY *.sln .

COPY Tests/Equinor.Procosys.Library.Test.Common/*.csproj ./Tests/Equinor.Procosys.Library.Test.Common/

COPY Equinor.Procosys.Library.Command/*.csproj ./Equinor.Procosys.Library.Command/
COPY Tests/Equinor.Procosys.Library.Command.Tests/*.csproj ./Tests/Equinor.Procosys.Library.Command.Tests/

COPY Equinor.Procosys.Library.Domain/*.csproj ./Equinor.Procosys.Library.Domain/
COPY Tests/Equinor.Procosys.Library.Domain.Tests/*.csproj ./Tests/Equinor.Procosys.Library.Domain.Tests/

COPY Equinor.Procosys.Library.Infrastructure/*.csproj ./Equinor.Procosys.Library.Infrastructure/
COPY Tests/Equinor.Procosys.Library.Infrastructure.Tests/*.csproj ./Tests/Equinor.Procosys.Library.Infrastructure.Tests/

COPY Equinor.Procosys.Library.Query/*.csproj ./Equinor.Procosys.Library.Query/
COPY Tests/Equinor.Procosys.Library.Query.Tests/*.csproj ./Tests/Equinor.Procosys.Library.Query.Tests/

COPY Equinor.Procosys.Library.WebApi/*.csproj ./Equinor.Procosys.Library.WebApi/
COPY Tests/Equinor.Procosys.Library.WebApi.Tests/*.csproj ./Tests/Equinor.Procosys.Library.WebApi.Tests/

# Restore NuGet packages
RUN dotnet restore

# Copy full solution
COPY . .

# Build
RUN dotnet build -c Release --no-restore

# Test runner target
FROM build AS testrunner
WORKDIR /src
RUN dotnet test -c Release --no-build --no-restore

# Publish the application
FROM build AS publish
WORKDIR "/src/Equinor.Procosys.Library.WebApi"
RUN dotnet publish "Equinor.Procosys.Library.WebApi.csproj" -c Release --no-restore -o /app/publish

# Define the image used for the final result
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
# Create non-root user. Set ui to 9999 to avoid conflicts with host OS just in case
RUN adduser --disabled-password --uid 9999 --gecos "" apprunner
# Create the folder and set the non-root as owner
RUN mkdir /app && chown apprunner.apprunner /app
# Change the user from root to non-root- From now on, all Docker commands are run as non-root user (except for COPY)
USER apprunner
# Set the port to 5000 since the default production port is 80 and on-root users cannot bind to this port
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000
WORKDIR /app
# Copy the published files from the build image into this one
# COPY defaults to copying files as root, specify the user that should be the owner
COPY --chown=apprunner:apprunner --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Equinor.Procosys.Library.WebApi.dll"]