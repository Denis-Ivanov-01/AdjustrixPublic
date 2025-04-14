using AdjustrixBase.Adjustment.Leveling;
using AdjustrixBase.DataLoading;
using AdjustrixBase.Project;

class Program
{
    static void Main(string[] args)
    {
        string folder = @"C:\\Users\\denis\\Desktop\\Геодезия\\_Дипломна";
        string file = @"C:\Users\denis\Desktop\Геодезия\_Дипломна\project.adjx";

        ProjectFileManager projectFile = new ProjectFileManager();
        //LevelingProject proj = (LevelingProject)projectFile.FromFile(file);

        JSONLevelingData data = new(@"C:\Users\denis\source\repos\Adjustrix\testInputData3.json");
        //LevelingAdjuster adjuster = new LevelingAdjuster(data.HeightDifferences);
        //adjuster.PerformAdjustment();
        LevelingProject project = new(
            data.HeightDifferences,
            data.KnownBenchmarks,
            //@"C:\\Users\\denis\\Desktop\\Геодезия\\_Дипломна",
            "project", "site", "contractor", "client");

        //projectFile.ToFile(project, folder);
        //LevelingProject levelingProject = (LevelingProject)projectFile.FromFile(file);
        //projectFile.ToFile(levelingProject, folder);
        LevelingProject project2 = (LevelingProject)projectFile.FromFile(file);

        LevelingProcessing processing = new(project2);
        processing.Process(folder);

    }
}
