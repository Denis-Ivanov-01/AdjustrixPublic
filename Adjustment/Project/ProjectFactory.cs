namespace Adjustment.Project
{
    public class ProjectFactory
    {
        public static AdjustrixProject CreateProject(ProjectCreationProperties properties, ProjectType projectType)
        {
            switch (projectType)
            {
                case ProjectType.Leveling:
                    List<HeightDifference> heightDifferences = new List<HeightDifference>();
                    return new LevelingProject(
                        heightDifferences,
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
