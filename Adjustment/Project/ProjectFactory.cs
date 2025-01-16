namespace Adjustment.Project
{
    public class ProjectFactory
    {
        public static AdjustrixProject CreateProject(ProjectCreationProperties properties, ProjectType projectType)
        {
            switch (projectType)
            {
                case ProjectType.Leveling:
                    List<HeightDelta> heightDifferences = new List<HeightDelta>();
                    HashSet<KnownBenchmark> knownBenchmarks = new();
                    return new LevelingProject(
                        heightDifferences,
                        knownBenchmarks,
                        properties.ProjectName,
                        properties.SiteName,
                        properties.Contractor,
                        properties.Client
                    );
                default:
                    throw new NotImplementedException("Not implemented project type in the project factory!");
            }
        }
    }
}
