#!/bin/bash
# Coverage report generation script for Linux/macOS
# Generates HTML coverage reports from Cobertura XML files

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
cd "$REPO_ROOT"

echo "Generating code coverage HTML reports..."

if ! dotnet tool list -g | grep -q "reportgenerator"; then
    echo "Installing reportgenerator tool..."
    dotnet tool install -g dotnet-reportgenerator-globaltool
    if [ $? -ne 0 ]; then
        echo "Failed to install reportgenerator tool!"
        exit 1
    fi
fi

COVERAGE_FILES=$(find TestResults -name "coverage.cobertura.xml" 2>/dev/null || true)

if [ -z "$COVERAGE_FILES" ]; then
    echo "No coverage files found in TestResults directory."
    echo "Please run tests first using: ./scripts/test.sh"
    exit 1
fi

FILE_COUNT=$(echo "$COVERAGE_FILES" | wc -l | tr -d ' ')
echo "Found $FILE_COUNT coverage file(s)"

OUTPUT_DIR="TestResults/coverage/html"
rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

COVERAGE_PATHS=$(echo "$COVERAGE_FILES" | tr '\n' ';' | sed 's/;$//')

reportgenerator \
    -reports:"$COVERAGE_PATHS" \
    -targetdir:"$OUTPUT_DIR" \
    -reporttypes:"Html"

if [ $? -ne 0 ]; then
    echo "Failed to generate coverage reports!"
    exit 1
fi

HTML_REPORT_PATH="$REPO_ROOT/$OUTPUT_DIR/index.html"
echo ""
echo "Coverage reports generated successfully!"
echo "HTML report location: $HTML_REPORT_PATH"
echo "Open the report in your browser to view coverage details."