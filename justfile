# Justfile for RoadStatus CLI
# A command runner for common development tasks

# Set shell for Windows compatibility
# This ensures just can find a shell on Windows systems
set shell := ["cmd.exe", "/c"]

# Default recipe - show available commands
default:
    @just --list

# Build the solution
build:
    dotnet build rs-cli.sln

# Build in Release configuration
build-release:
    dotnet build rs-cli.sln --configuration Release

# Run all tests
test:
    dotnet test rs-cli.sln

# Run tests with code coverage collection
coverage:
    dotnet test rs-cli.sln \
        --collect:"XPlat Code Coverage" \
        --results-directory:"TestResults" \
        --logger:"console;verbosity=normal" \
        -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
    @echo ""
    @echo "Coverage data saved to TestResults directory"
    @echo "To generate HTML reports, run: just coverage-html"

# Generate HTML coverage reports from collected coverage data
# Note: This recipe uses PowerShell syntax. On Windows, ensure PowerShell is available.
# On Linux/macOS, you may need to use the generate-coverage-report.sh script instead.
coverage-html:
    powershell -Command "if (-not (dotnet tool list -g | Select-String 'reportgenerator')) { dotnet tool install -g dotnet-reportgenerator-globaltool }; reportgenerator -reports:'TestResults/**/coverage.cobertura.xml' -targetdir:'TestResults/coverage/html' -reporttypes:'Html'; Write-Host ''; Write-Host 'HTML coverage report generated at: TestResults/coverage/html/index.html'"

# Format code using .NET format tool
format:
    dotnet format rs-cli.sln

# Format code and verify (dry-run)
format-check:
    dotnet format rs-cli.sln --verify-no-changes