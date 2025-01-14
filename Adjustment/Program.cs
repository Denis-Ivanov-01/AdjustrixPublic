using Adjustment;
using Adjustment.Adjustment;
using Adjustment.Project;

class Program
{
    static void Main(string[] args)
    {
        JSONLevelingData data = new(@"C:\Users\denis\source\repos\Adjustrix\testInputData2.json");
        //LevelingAdjuster adjuster = new LevelingAdjuster(data.HeightDifferences);
        //adjuster.PerformAdjustment();
        LevelingProject project = new(
            data.HeightDifferences,
            @"C:\\Users\\denis\\Desktop\\Геодезия\\_Дипломна",
            "project", "site", "contractor", "client");
        ProjectFile projectFile = new ProjectFile();
        projectFile.ToFile(project);
        LevelingProject levelingProject = (LevelingProject)projectFile.FromFile(@"C:\Users\denis\Desktop\Геодезия\_Дипломна\project.adjx");
        projectFile.ToFile(levelingProject);
        LevelingProject project2 = (LevelingProject)projectFile.FromFile(@"C:\Users\denis\Desktop\Геодезия\_Дипломна\project.adjx");
        Console.WriteLine();
    }
}
