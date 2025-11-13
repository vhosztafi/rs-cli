# RoadStatus CLI

A .NET console application that queries the TfL Road API to display road status information.

## Overview

This application queries the Transport for London (TfL) Road API to retrieve and display the status of major roads. Given a valid road ID, it displays the road's display name, status severity, and status description. For invalid road IDs, it returns an informative error message and exits with a non-zero error code.

## Build

```bash
dotnet build
```

## Run

The application works without API credentials, but setting them enables higher rate limits (500 requests/minute). To use credentials, set them as environment variables:

```bash
export TFL_APP_ID=your_app_id
export TFL_APP_KEY=your_app_key
```

Then run:

```bash
dotnet run --project src/RoadStatus.Cli -- <road-id>
```

## Examples

### Success case (A2)

```bash
dotnet run --project src/RoadStatus.Cli -- A2
```

Output:
```
The status of the A2 is as follows
        Road Status is Good
        Road Status Description is No Exceptional Delays
```

Exit code: `0`

### Error case (A233)

```bash
dotnet run --project src/RoadStatus.Cli -- A233
```

Output:
```
A233 is not a valid road
```

Exit code: `1`

### Check exit code (PowerShell)

```powershell
dotnet run --project src/RoadStatus.Cli -- A2
$lastexitcode
```

## Tests

### Running Tests

Run all tests:

```bash
dotnet test
```

Or use the provided test scripts that include code coverage collection:

- **Windows (PowerShell)**:
  ```powershell
  .\scripts\test.ps1
  ```

- **Linux/macOS (Bash)**:
  ```bash
  ./scripts/test.sh
  ```

### Code Coverage

Code coverage is automatically collected when using the test scripts. Coverage data is saved to the `TestResults` directory in the coverage format.

To view coverage reports, you can use tools like:
- Visual Studio Code Coverage viewer
- Other coverage analysis tools that support the XPlat Code Coverage format

### Integration Tests

Integration tests are opt-in. Set `RUN_LIVE_INTEGRATION=1` along with `TFL_APP_ID` and `TFL_APP_KEY` to run live API tests.

## Configuration

### API Keys

The TfL Road API can be used without authentication, but authenticated requests benefit from higher rate limits (500 requests/minute vs. lower limits for unauthenticated requests).

To use API credentials:

1. **Obtain API keys**: Register for a free TfL API account at [https://api.tfl.gov.uk/](https://api.tfl.gov.uk/) to get your `app_id` and `app_key`.

2. **Set environment variables**:
   - **Windows (PowerShell)**:
     ```powershell
     $env:TFL_APP_ID="your_app_id_here"
     $env:TFL_APP_KEY="your_app_key_here"
     ```
   - **Windows (CMD)**:
     ```cmd
     set TFL_APP_ID=your_app_id_here
     set TFL_APP_KEY=your_app_key_here
     ```
   - **Linux/macOS (Bash)**:
     ```bash
     export TFL_APP_ID=your_app_id_here
     export TFL_APP_KEY=your_app_key_here
     ```

3. **Security note**: Never commit API keys to version control. Use environment variables, secure configuration files, or secret management systems in production environments.

### Base URL

The application defaults to `https://api.tfl.gov.uk` but can be configured via the `TflRoadStatusClient` constructor for testing or custom endpoints.

## Assumptions

- **API Credentials**: TfL API credentials (`TFL_APP_ID` and `TFL_APP_KEY`) are optional. The API works without them but with lower rate limits.

- **Network Connectivity**: Network connectivity is available to reach `https://api.tfl.gov.uk`. The application requires internet access to query the TfL Road API.

- **Road ID Format**: Road IDs are case-insensitive (the API handles this). Valid road IDs follow TfL's naming conventions (e.g., "A2", "A233").

- **JSON Response Format**: The application expects JSON responses with properties `displayName`, `statusSeverity`, and `statusSeverityDescription` (case-insensitive matching is supported).

- **Runtime**: The application targets .NET 8.0 and requires the .NET 8.0 runtime or SDK to build and run.

- **Single Road Query**: The application queries one road at a time. Multiple road IDs are not supported in a single invocation.
