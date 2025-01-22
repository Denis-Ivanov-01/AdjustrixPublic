using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Adjustment;
using ExcelDataReader;

namespace AdjustrixWPF.Model
{
    public class LevelingExcelReader
    {
        private const string benchmarksSheet = "KnownBenchmarks";
        private const string measurementsSheet = "Measurements";

        private const string number = "Number";
        private const string elevation = "Elevation";

        private const string fromPoint = "FromPoint";
        private const string toPoint = "ToPoint";
        private const string length = "Length";
        private const string value = "Value";

        private bool benchmarksParsed = false;

        private string excelPath;

        public HashSet<KnownBenchmark> KnownBenchmarks { get; private set; } = new();

        public List<HeightDelta> Measurements { get; private set; } = new();

        public LevelingExcelReader(string excelPath)
        {
            this.excelPath = excelPath;
            DataSet excel = ReadExcel();
            ParseExcel(excel);
        }

        private void ParseExcel(DataSet excel)
        {
            (DataTable, DataTable) result = GetSheets(excel);
            DataTable knownBenchmarks = result.Item1;
            DataTable measurements = result.Item2;

            ParseBenchmarks(knownBenchmarks);
            ParseMeasurements(measurements);
        }

        private (DataTable, DataTable) GetSheets(DataSet excel)
        {
            DataTable? knownBenchmarks = null;
            DataTable? measurements = null;
            foreach (DataTable t in excel.Tables)
            {
                if (t.TableName == benchmarksSheet)
                {
                    knownBenchmarks = t;
                }
                else if (t.TableName == measurementsSheet)
                {
                    measurements = t;
                }
            }
            if (measurements == null || knownBenchmarks == null)
            {
                throw new ArgumentNullException("The passed Excel file did not contain the required sheets");
            }
            return (knownBenchmarks, measurements);
        }

        private void ParseBenchmarks(DataTable benchmarks)
        {
            int numIndex = benchmarks.Columns.IndexOf(number);
            int elevIndex = benchmarks.Columns.IndexOf(elevation);

            foreach (DataRow row  in benchmarks.Rows)
            {
                string num = (string)row[numIndex];
                double val = (double)row[elevIndex];
                KnownBenchmarks.Add(new(num, val));
            }

            benchmarksParsed = true;
        }

        private void ParseMeasurements(DataTable measurements)
        {
            if (!benchmarksParsed) 
            { 
                throw new ArgumentNullException("The benchmarks from the Excel must be parsed before the measurements"); 
            }

            int fromIndex = measurements.Columns.IndexOf(fromPoint);
            int toIndex = measurements.Columns.IndexOf(toPoint);
            int lengthIndex = measurements.Columns.IndexOf(length);
            int valueIndex = measurements.Columns.IndexOf(value);

            foreach (DataRow row in measurements.Rows)
            {
                string fromPointNum = (string)row[fromIndex];
                string toPointNum = (string)row[toIndex];
                double length = (double)row[lengthIndex];
                double value = (double)row[valueIndex];

                PointBase fromPoint = PointFromString(fromPointNum);
                PointBase toPoint = PointFromString(toPointNum);

                Measurements.Add(new(fromPoint, toPoint, value, length));
            }
        }

        private PointBase PointFromString(string number)
        {
            HashSet<KnownBenchmark> filtered = KnownBenchmarks.Where(kb => kb.Number == number).ToHashSet();
            if (filtered.Count == 1)
            {
                return filtered.First();
            }
            return new NewBenchmark(number);
        }

        private DataSet ReadExcel()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            DataSet result;
            using (FileStream stream = new FileStream(excelPath,  FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    result = reader.AsDataSet(new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true // Set to true if the first row contains column names
                        }
                    });
                }
            }

            return result;
        }

    }
}
