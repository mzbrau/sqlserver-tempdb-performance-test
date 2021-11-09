# SQL Server Temp DB Performance Test Tool

## Introduction
During testing we have discovered a potential bug in SQL Server 2017 when using the TEMP DB table.
Under sustained load in the tempDB for 12 hours the performance gradually degrades.
This tool is able to generate this condition and outputs some basic metrics to be able to chart the progress.

## Building the Tool
To build the tool, perform a dotnet publish. It is designed to build into a single file executable.

```
dotnet publish -r win-x64 -c release
```

## Running the Tool
The tool has the following arguments:

- thread count - the number of threads that should be running queries simultaneously.
- delay between queries - the delay in ms between a thread performing each query.
- item count - the number of items that should be added to the tempdb for each query.
- connection string - the connection string to the SQL server.

Arguments are provided in the following order:
```
TempDbPerformanceTester.exe <thread count> <delay between queries in ms> <item count> <connection string>"
```

For example:
```
TempDbPerformanceTester.exe 20 10 100 "Server=TESTLISTENER,1433;Database=TestDb2;Integrated Security=True"
```

It is also suggested to port the result to file:
```
TempDbPerformanceTester.exe 20 10 100 "Server=TESTLISTENER,1433;Database=TestDb2;Integrated Security=True" > Report.txt
```