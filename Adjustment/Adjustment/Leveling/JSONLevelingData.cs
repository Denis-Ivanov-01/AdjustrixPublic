using System.Text.Json;

namespace Adjustment.Adjustment
{

    public class HeightDifferenceJSON
    {
        public string FromPoint { get; set; }

        public string ToPoint { get; set; }

        public double Value { get; set; }

        public double Length { get; set; }
    }

    public class KnownBenchmarkJSON
    {
        public string Number { get; set; }

        public double Elevation { get; set; }
    }

    public class LevelingData
    {
        public List<KnownBenchmarkJSON> KnownBenchmarks { get; set; }

        public List<HeightDifferenceJSON> ElevationDifferences { get; set; }
    }

    public class JSONLevelingData
    {
        public LevelingData LevelingData { get; }
        
        public HashSet<KnownBenchmark> KnownBenchmarks = new();
        private bool knownBenchmarksExtracted = false;

        public HashSet<NewBenchmark> NewBenchmarks = new();
        private bool newBenchmarksExtracted = false;

        public List<HeightDifference> HeightDifferences = new();

        public JSONLevelingData(string jsonPath)
        {
            LevelingData = ParseFile(jsonPath);
            GetKnownBenchmarks();
            GetNewBenchmarks();
            GetMeasurements();
        }

        private static LevelingData ParseFile(string jsonPath)
        {
            string jsonContent = File.ReadAllText(jsonPath);
            return JsonSerializer.Deserialize<LevelingData>(jsonContent)!;
        }

        private void GetKnownBenchmarks()
        {
            foreach (KnownBenchmarkJSON benchmarkJSON in LevelingData.KnownBenchmarks)
            {
                KnownBenchmarks.Add(new KnownBenchmark(benchmarkJSON.Number, benchmarkJSON.Elevation));
            }
            knownBenchmarksExtracted = true;
        }

        private void GetNewBenchmarks()
        {
            if (!knownBenchmarksExtracted) { throw new ArgumentNullException("Known benchmarks not extracted!"); }
            foreach (HeightDifferenceJSON meas in LevelingData.ElevationDifferences)
            {
                AddNewBenchmark(meas.FromPoint);
                AddNewBenchmark(meas.ToPoint);
            }
            newBenchmarksExtracted = true;
        }

        private void AddNewBenchmark(string number)
        {
            if (NewBenchmarks.Any(x => x.Number == number) || KnownBenchmarks.Any(x => x.Number == number)) 
            { return; }
            NewBenchmarks.Add(new NewBenchmark(number));
        }

        private void GetMeasurements()
        {
            if (!newBenchmarksExtracted || !knownBenchmarksExtracted)
            {
                throw new ArgumentNullException("Cannot extract the measurements before extracting the benchmarks!");
            }

            foreach (HeightDifferenceJSON meas in LevelingData.ElevationDifferences)
            {
                PointBase fromPoint = GetPointByNumber(meas.FromPoint);
                PointBase toPoint = GetPointByNumber(meas.ToPoint);
                HeightDifferences.Add(new HeightDifference(fromPoint, toPoint, meas.Value, meas.Length));
            }
        }

        private PointBase GetPointByNumber(string number)
        {
            IEnumerable<KnownBenchmark> filtered = KnownBenchmarks.Where(x => x.Number == number);
            if (filtered.Any())
            {
                return filtered.First();
            }
            return NewBenchmarks.Where(x => x.Number == number).First();
        }
    }
}
