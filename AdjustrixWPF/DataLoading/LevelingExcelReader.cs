using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AdjustrixBase.DataModels;
using ExcelDataReader;

namespace AdjustrixWPF.DataLoading
{
    public class LevelingExcelReader
    {
        private const string BenchmarksSheetName = "KnownBenchmarks";
        private const string MeasurementsSheetName = "Measurements";

        private const string ColumnNumber = "Number";
        private const string ColumnElevation = "Elevation";

        private const string ColumnFromPoint = "FromPoint";
        private const string ColumnToPoint = "ToPoint";
        private const string ColumnLength = "Length";
        private const string ColumnValue = "Value";

        private bool benchmarksParsed = false;

        public HashSet<KnownBenchmark> KnownBenchmarks { get; private set; } = new();
        public List<HeightDelta> Measurements { get; private set; } = new();

        private readonly string excelPath;

        public LevelingExcelReader(string excelPath)
        {
            if (string.IsNullOrWhiteSpace(excelPath) || !File.Exists(excelPath))
            {
                throw new ArgumentException("Invalid Excel file path", nameof(excelPath));
            }

            this.excelPath = excelPath;
            DataSet excelData = ReadExcel();
            ParseExcel(excelData);
        }

        private void ParseExcel(DataSet excelData)
        {
            var sheets = GetSheets(excelData);
            ParseBenchmarks(sheets.benchmarksSheet);
            ParseMeasurements(sheets.measurementsSheet);
        }

        private (DataTable benchmarksSheet, DataTable measurementsSheet) GetSheets(DataSet excelData)
        {
            var benchmarksSheet = excelData.Tables.Cast<DataTable>().FirstOrDefault(t => t.TableName == BenchmarksSheetName);
            var measurementsSheet = excelData.Tables.Cast<DataTable>().FirstOrDefault(t => t.TableName == MeasurementsSheetName);

            if (benchmarksSheet == null || measurementsSheet == null)
            {
                throw new InvalidOperationException("The Excel file must contain both 'KnownBenchmarks' and 'Measurements' sheets.");
            }

            return (benchmarksSheet, measurementsSheet);
        }

        private void ParseBenchmarks(DataTable benchmarks)
        {
            if (!benchmarks.Columns.Contains(ColumnNumber) || !benchmarks.Columns.Contains(ColumnElevation))
            {
                throw new InvalidOperationException("The 'KnownBenchmarks' sheet is missing required columns.");
            }

            foreach (DataRow row in benchmarks.Rows)
            {
                string number = row[ColumnNumber]?.ToString() ?? throw new InvalidOperationException("Benchmark number is missing.");
                if (!double.TryParse(row[ColumnElevation]?.ToString(), out double elevation))
                {
                    throw new InvalidOperationException($"Invalid elevation value for benchmark {number}.");
                }
                number = number.Trim();
                KnownBenchmarks.Add(new KnownBenchmark(number, elevation));
            }

            benchmarksParsed = true;
        }

        private void ParseMeasurements(DataTable measurements)
        {
            if (!benchmarksParsed)
            {
                throw new InvalidOperationException("ParseBenchmarks must be called before ParseMeasurements.");
            }

            var requiredColumns = new[] { ColumnFromPoint, ColumnToPoint, ColumnLength, ColumnValue };
            foreach (var column in requiredColumns)
            {
                if (!measurements.Columns.Contains(column))
                {
                    throw new InvalidOperationException($"The 'Measurements' sheet is missing the required column: {column}.");
                }
            }

            foreach (DataRow row in measurements.Rows)
            {
                string fromPoint = row[ColumnFromPoint]?.ToString() ?? throw new InvalidOperationException("FromPoint is missing.");
                string toPoint = row[ColumnToPoint]?.ToString() ?? throw new InvalidOperationException("ToPoint is missing.");
                if (!double.TryParse(row[ColumnLength]?.ToString(), out double length))
                {
                    throw new InvalidOperationException($"Invalid length value for measurement from {fromPoint} to {toPoint}.");
                }
                if (!double.TryParse(row[ColumnValue]?.ToString(), out double value))
                {
                    throw new InvalidOperationException($"Invalid value for measurement from {fromPoint} to {toPoint}.");
                }

                Measurements.Add(new HeightDelta(PointFromString(fromPoint.Trim()), PointFromString(toPoint.Trim()), value, length));
            }
        }

        private PointBase PointFromString(string pointNumber)
        {
            if (KnownBenchmarks.FirstOrDefault(kb => kb.Number == pointNumber) is KnownBenchmark benchmark)
            {
                return benchmark;
            }

            return new NewBenchmark(pointNumber);
        }

        private DataSet ReadExcel()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using FileStream stream = new(excelPath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            return reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            });
        }
    }
}
