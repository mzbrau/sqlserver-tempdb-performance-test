# SQL Server Temp DB Performance Test Tool

## Introduction

During testing we have discovered a potential bug in SQL Server 2017 when using the TEMP DB table.
Under sustained load in the tempDB for 12 hours the performance gradually degrades.
This tool is able to generate this condition and outputs some basic metrics to be able to chart the progress.

## Building the Tool

To build the tool, perform a dotnet publish. It is designed to build into a single file executable.

```text
dotnet publish -r win-x64 -c release
```

## Running the Tool

The tool has the following arguments:

- thread count - the number of threads that should be running queries simultaneously.
- delay between queries - the delay in ms between a thread performing each query.
- item count - the number of items that should be added to the tempdb for each query.
- connection string - the connection string to the SQL server.

Arguments are provided in the following order:

```text
TempDbPerformanceTester.exe <thread count> <delay between queries in ms> <item count> <connection string>"
```

For example:

```text
TempDbPerformanceTester.exe 20 10 100 "Server=TESTLISTENER,1433;Database=TestDb2;Integrated Security=True"
```

It is also suggested to port the result to file:

```text
TempDbPerformanceTester.exe 20 10 100 "Server=TESTLISTENER,1433;Database=TestDb2;Integrated Security=True" > Report.txt
```

## What the Tool Does

1. Starts up X threads (20 in the above example)
2. Each thread
   1. Connects to the database
   2. Declares a table
   3. Generates ~100 random numbers
   4. Inserts them into the table
   5. Does an insert from the table into a tempDB table
   6. Performs update statistics on the tempDB
   7. Does a basic query on the tempDB table
   8. Drops the tempDB table
   9. Disconnects from the database
   10. Logs out how long it took (only every 1000 iterations)
   11. Waits (10ms in this case)
   12. Starts again at step a

## Results

### SQL 2017 CU27

This tool was run for 16 hours against SQL 2017 CU27. The result was then charted in Excel.
The raw data for this can be found [here](https://github.com/mzbrau/sqlserver-tempdb-performance-test/blob/main/Resources/SQL2017_RawData.xlsx).

#### Tool Output

![excel chart of tool output](https://github.com/mzbrau/sqlserver-tempdb-performance-test/blob/main/Resources/SQL2017_ToolOutput_QueryDuration.png)

#### Query Store Results

![excel chart of query store execution time](https://github.com/mzbrau/sqlserver-tempdb-performance-test/blob/main/Resources/SQL2017_QueryStore_QueryDuration.png)

![excel chart of query store CPU usage](https://github.com/mzbrau/sqlserver-tempdb-performance-test/blob/main/Resources/SQL2017_QueryStore_CPUTime.png)

#### Analysis

It is clear from both the tool output chart and the query store results that there is an upward trend on the execution time of this query. This suggests that SQL server is not correctly cleaning up some resources at the end of each query and these build up over time and result in slower results. This is the latest cumulative update patch for SQL 2017 as of time of writing (Nov 2021).