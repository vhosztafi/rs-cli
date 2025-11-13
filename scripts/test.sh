#!/bin/bash
# Test script for Linux/macOS
# Runs all tests with code coverage collection

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
cd "$REPO_ROOT"

echo "Running tests with code coverage..."

dotnet test \
    --collect:"XPlat Code Coverage" \
    --results-directory:"TestResults" \
    --logger:"console;verbosity=normal"

if [ $? -ne 0 ]; then
    echo "Tests failed!"
    exit 1
fi

echo ""
echo "Tests completed successfully!"
echo "Coverage data saved to TestResults directory"