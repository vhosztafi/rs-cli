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

```bash
dotnet test
```

Integration tests are opt-in. Set `RUN_LIVE_INTEGRATION=1` along with `TFL_APP_ID` and `TFL_APP_KEY` to run live API tests.

## Assumptions

- TfL API credentials (`TFL_APP_ID` and `TFL_APP_KEY`) are optional - the API works without them but with lower rate limits
- Network connectivity is available to reach `https://api.tfl.gov.uk`
- Road IDs are case-insensitive (API handles this)
- The application targets .NET 8.0
