

using System.Text;

namespace Adjustment.Reports.Leveling
{
    public class AdjustedPointsReport : IReportGenerator
    {
        private const string name = "AdjustedBenchmarks";
        private const string extension = ".csv";

        private readonly List<AdjustedBenchmark> adjustedBenchmarks;

        public AdjustedPointsReport(List<AdjustedBenchmark> benchmarks)
        {
            adjustedBenchmarks = benchmarks;
        }

        public void Generate(string dir)
        {
            string name = GetName(dir);
            string fullName = Path.ChangeExtension(name, extension);
            string path = Path.Combine(dir, fullName);
            string initialLine = "number, elevation, variance_milimeters";
            string[] lines = new string[adjustedBenchmarks.Count + 1];
            lines[0] = initialLine;
            for (int i = 0; i < adjustedBenchmarks.Count; i++)
            {
                AdjustedBenchmark b = adjustedBenchmarks[i];
                lines[i + 1] = string.Join(", ", b.Number, Math.Round(b.Value, 4), Math.Round(b.Variance, 2));
            }

            byte[] bom = new byte[] { 0xEF, 0xBB, 0xBF }; // UTF-8 BOM
            byte[] contentBytes = Encoding.UTF8.GetBytes(string.Join("\n", lines));
            byte[] bytes = bom.Concat(contentBytes).ToArray();

            File.WriteAllBytes(path, bytes);
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
                finalName = $"{name}_{counter}";
                counter++;
            }
            return finalName;
        }
    }
}