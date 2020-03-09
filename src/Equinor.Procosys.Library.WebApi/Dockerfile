FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
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

# Create a runtime image and copy the application over
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Equinor.Procosys.Library.WebApi.dll"]