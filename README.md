# RoadStatus CLI

A .NET console application that queries the TfL Road API to display road status information.

## Overview

This application queries the Transport for London (TfL) Road API to retrieve and display the status of major roads. Given a valid road ID, it displays the road's display name, status severity, and status description. For invalid road IDs, it returns an informative error message and exits with a non-zero error code.

## Build

```bash
dotnet build
```

## Run

```bash
dotnet run --project src/RoadStatus.Cli -- <road-id>
```

## Tests

```bash
dotnet test
```

