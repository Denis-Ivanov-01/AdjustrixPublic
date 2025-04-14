using AdjustrixBase.DataModels;

namespace AdjustrixBase.Extensions
{
    class Data
    {
        public List<HeightDelta> Measurements { get; set; }

        public HashSet<Benchmark> Benchmarks { get; set; }
    }

    internal interface IDataReader
    {//todo: implement for excel and Json - both in base
        public Data ReadData();
    }
}
