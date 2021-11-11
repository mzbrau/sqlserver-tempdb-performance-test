using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace TempDbPerformanceTester
{
    public class TempDbQueryWorker
    {
        private readonly int _id;
        private readonly Random _random;
        private readonly int _delay;
        private readonly int _itemCount;
        private readonly string _connectionString;

        public TempDbQueryWorker(int id, Random random, int delay, int itemCount, string connectionString)
        {
            _id = id;
            _random = random;
            _delay = delay;
            _itemCount = itemCount;
            _connectionString = connectionString;
        }

        public void Run()
        {
            Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.InvariantCulture)} Starting query worker {_id}. Connection string is '{_connectionString}'");
            long queryCount = 0;
            var watch = Stopwatch.StartNew();
            while (true)
            {
                try
                {
                    QueryTempDb();
                    queryCount++;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{_id}: Error running: {e}");
                }

                if (queryCount % 1000 == 0)
                {
                    Console.WriteLine($"{_id},{DateTime.Now.ToString("g", CultureInfo.InvariantCulture)},{queryCount} Queries,{watch.ElapsedMilliseconds}");
                    queryCount = 0;
                    watch.Restart();
                }

                Thread.Sleep(_delay);
            }
        }

        private void QueryTempDb()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(CreateQuery(), connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("g", CultureInfo.InvariantCulture)} {_id}: Error running query: {e.Message}");
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        private IEnumerable<int> GetParameterIds()
        {
            var result = new List<int>();
            for (int i = 0; i < _itemCount; i++)
            {
                result.Add(_random.Next(999999));
            }

            return result.Distinct();
        }

        private string CreateQuery()
        {
            var queryBuilder = new StringBuilder();

            queryBuilder.AppendLine("declare @p999 table (id bigint not null)");
            foreach (var id in GetParameterIds())
            {
                queryBuilder.AppendLine($"insert into @p999 values({id})");
            }
            queryBuilder.AppendLine(";SELECT * INTO #PERFTEST FROM @p999;");
            queryBuilder.AppendLine("UPDATE STATISTICS #PERFTEST;");
            queryBuilder.AppendLine("SELECT * from #PERFTEST;");
            queryBuilder.AppendLine("DROP TABLE #PERFTEST");

            return queryBuilder.ToString();
        }
    }
}