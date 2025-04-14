using AdjustrixBase.DataModels;

namespace AdjustrixBase.Project
{
    public class ProjectFactory
    {//todo: not really a factory, think of a better name
        public static AdjustrixProject CreateProject(ProjectGeneralProperties properties, ProjectType projectType)
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
