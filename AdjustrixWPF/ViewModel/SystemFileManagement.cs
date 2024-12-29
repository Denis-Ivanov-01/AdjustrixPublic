using System;
using System.IO;

namespace AdjustrixWPF.ViewModel
{
    public class SystemFileManagement
    {
        private static readonly string programFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
        private static readonly string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            .Replace("Roaming", "Local");
        private const string adjustrixFolder = "Adjustrix";

        private static readonly string adjustrixPFfolder = Path.Combine(programFiles, adjustrixFolder);

        private static readonly string adjustrixADfolder = Path.Combine(appData, adjustrixFolder);
        private static readonly string appearanceFolder = Path.Combine(adjustrixADfolder, "Appearance");
        private static readonly string languageFile = Path.Combine(appearanceFolder, "language.txt");
        private static readonly string themeFile = Path.Combine(appearanceFolder, "theme.txt");

        private static SystemFileManagement instance;

        public static SystemFileManagement Singleton => GetSingleton();

        private SystemFileManagement()
        {
            EnsurePFFolderExists();
            Directory.CreateDirectory(appearanceFolder);
            EnsureAppearanceFilesExist();
        }

        private static SystemFileManagement GetSingleton()
        {
            if (instance == null)
            {
                instance = new SystemFileManagement();
            }
            return instance;
        }

        public string LanguageString
        {
            get
            {
                return File.ReadAllText(languageFile);
            }
            set
            {
                File.WriteAllText(languageFile, value);
            }
        }

        public string ThemeString
        {
            get
            {
                return File.ReadAllText(themeFile);
            }
            set
            {
                File.WriteAllText(themeFile, value);
            }
        }

        private void EnsureAppearanceFilesExist()
        {
            CreateFileIfNotExists(languageFile, "English");
            CreateFileIfNotExists(themeFile, "Dark");
        }

        private static void CreateFileIfNotExists(string path, string content = "")
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, content);
            }
        }

        private void EnsurePFFolderExists()
        {
            if (!Directory.Exists(adjustrixPFfolder))
            {//todo: think of a good message to display to the user in that case!
                throw new DirectoryNotFoundException($"The system folder {adjustrixPFfolder} was not found!");
            }
        }
    }
}
