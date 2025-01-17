

namespace Adjustment.Reports.Leveling
{
    public class AdjustedPointsReport : IReportGenerator
    {
        private const string name = "AdjustedBenchmarks";
        private const string extension = ".csv";

        private List<AdjustedBenchmark> adjustedBenchmarks;

        public AdjustedPointsReport(List<AdjustedBenchmark> benchmarks)
        {
            adjustedBenchmarks = benchmarks;
        }

        public void Generate(string dir)
        {
            string name = GetName(dir);
            string fullName = Path.ChangeExtension(name, extension);
            string path = Path.Combine(dir, fullName);
            string initialLine = "number, elevation, error";
            string[] lines = new string[adjustedBenchmarks.Count + 1];
            lines[0] = initialLine;
            for (int i = 0; i < adjustedBenchmarks.Count; i++)
            {
                AdjustedBenchmark b = adjustedBenchmarks[i];
                lines[i + 1] = string.Join(", ", b.Number, b.Value, -1);
            }
            File.WriteAllLines(path, lines);
        }

        private string GetName(string dir)
        {
            string[] files = Directory.GetFiles(dir);
            string[] names = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                names[i] = Path.GetFileNameWithoutExtension(files[i]);
            }

            int counter = 1;
            string finalName = name;
            while (names.Contains(finalName))
            {
                finalName = $"{finalName}_{counter}";
                counter++;
            }
            return finalName;
        }
    }
}