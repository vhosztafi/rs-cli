#!/bin/bash
# Install git hooks by configuring git to use the .githooks directory

echo "Installing git hooks..."
echo ""

if [ ! -d ".githooks" ]; then
    echo "❌ Error: .githooks directory not found!"
    echo "Please run this script from the repository root."
    exit 1
fi

chmod +x .githooks/commit-msg
chmod +x .githooks/pre-push

git config core.hooksPath .githooks

if [ $? -eq 0 ]; then
    echo "✅ Git hooks installed successfully!"
    echo ""
    echo "Hooks configured:"
    echo "  - commit-msg: Validates conventional commit messages"
    echo "  - pre-push: Runs unit tests before pushing"
    echo ""
    echo "To uninstall, run: git config --unset core.hooksPath"
else
    echo "❌ Failed to install git hooks"
    exit 1
fi