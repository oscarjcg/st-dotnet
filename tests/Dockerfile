FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY st-dotnet/st-dotnet.csproj st-dotnet/
COPY tests/st-dotnet.UnitTests/st-dotnet.UnitTests.csproj tests/st-dotnet.UnitTests/
COPY tests/st-dotnet.UITests/st-dotnet.UITests.csproj tests/st-dotnet.UITests/
RUN dotnet restore tests/st-dotnet.UnitTests/st-dotnet.UnitTests.csproj \
 && dotnet restore tests/st-dotnet.UITests/st-dotnet.UITests.csproj

COPY st-dotnet/ st-dotnet/
COPY tests/ tests/

FROM build AS unit
RUN dotnet build tests/st-dotnet.UnitTests -c Release --no-restore -nologo
ENTRYPOINT ["dotnet", "test", "tests/st-dotnet.UnitTests", "-c", "Release", "--no-build", "-nologo", \
            "--logger", "trx;LogFileName=unit.trx", "--logger", "console;verbosity=normal", \
            "--results-directory", "/results"]

FROM build AS ui
RUN dotnet build tests/st-dotnet.UITests -c Release --no-restore -nologo
RUN pwsh tests/st-dotnet.UITests/bin/Release/net10.0/playwright.ps1 install --with-deps chromium
ENV RESULTS_DIR=/results
ENTRYPOINT ["dotnet", "test", "tests/st-dotnet.UITests", "-c", "Release", "--no-build", "-nologo", \
            "--logger", "trx;LogFileName=ui.trx", "--logger", "console;verbosity=normal", \
            "--results-directory", "/results"]
