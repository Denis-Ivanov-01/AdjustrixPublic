using System.Text.Json.Serialization;
using AdjustrixBase.DataModels;
using AdjustrixBase.Project;

namespace AdjustrixBase.Project
{
    public class LevelingProject : AdjustrixProject
    {
        private const ProjectType projectType = ProjectType.Leveling;

        public override string NetworkType
        {
            get
            {
                return projectType.ToString();
            }
        }

        public List<HeightDelta> HeightDifferences { get; set; }

        public HashSet<KnownBenchmark> KnownBenchmarks { get; set; }

        //private Dictionary<string, PointBase> benchmarksDict;

        [JsonConstructor]
        public LevelingProject(
            List<HeightDelta> HeightDifferences,
            HashSet<KnownBenchmark> KnownBenchmarks,
            //string ProjectFolder,
            string Name,
            string SiteName,
            string Contractor,
            string Client
            //ProjectType ProjectType = ProjectType.Leveling,
            //DateTime? CreateDate = null,
            //DateTime? LastUpdateDate = null
            )
            : base(Name, SiteName, Contractor, Client/*, CreateDate, LastUpdateDate*/)
        {
            //if (ProjectType != ProjectType.Leveling) { throw new ArgumentException("Incorrect project type!"); }
            this.HeightDifferences = HeightDifferences;
            this.KnownBenchmarks = KnownBenchmarks;
            EnsureValidDataTypes();
        }

        public override void EnsureValidDataTypes()
        { //TODO: think whether to leave this that way?
            //maybe it is a good idea to have some validation
            foreach (KnownBenchmark kb in KnownBenchmarks)
            {
                foreach (HeightDelta delta in HeightDifferences)
                {
                    if (delta.FromPoint.Number == kb.Number)
                    {
                        delta.FromPoint = kb;
                    }
                    else if (delta.ToPoint.Number == kb.Number)
                    {
                        delta.ToPoint = kb;
                    }
                }
            }
            foreach (HeightDelta delta in HeightDifferences)
            {
                Type fpType = delta.FromPoint.GetType();
                Type tpType = delta.ToPoint.GetType();
                if (fpType != typeof(KnownBenchmark) && fpType != typeof(NewBenchmark))
                {
                    delta.FromPoint = new NewBenchmark(delta.FromPoint.Number);
                }
                if (tpType != typeof(KnownBenchmark) && tpType != typeof(NewBenchmark))
                {
                    delta.ToPoint = new NewBenchmark(delta.ToPoint.Number);
                }
            }
        }
    }
}
