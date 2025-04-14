using System;
using System.IO;

namespace AdjustrixWPF.SystemManagement
{
    /// <summary>
    /// Intended to handle logic related to ProgramFiles and AppData
    /// There will be a setup.exe that will prepare the ProgramFiles directory in advance.
    /// </summary>
    public class SystemFileManagement
    {
        //todo: In the future add logic to handle Program files resources:
        // - Appearance folder for image loading - the logo of the program, an icon for the project file
        // - Runtime folder for a Python executable

        private static readonly string programFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
        private static readonly string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            .Replace("Roaming", "Local");
        private const string adjustrixFolder = "Adjustrix";

        private static readonly string adjustrixPFfolder = Path.Combine(programFiles, adjustrixFolder);
        //private static readonly string adjustrixPFview = Path.Combine(adjustrixPFfolder, "view");
        private static readonly string adjustrixDesktop = Path.Combine(adjustrixPFfolder, "Desktop");
        private static readonly string adjustrixAppearance = Path.Combine(adjustrixDesktop, "appearance");
        private static readonly string adjustrixRuntime = Path.Combine(adjustrixDesktop, "runtime");

        private static readonly string pythonExecutable = Path.Combine(adjustrixRuntime, "pythonw.exe");

        private static readonly string adjustrixADfolder = Path.Combine(appData, adjustrixFolder);
        private static readonly string appearanceFolder = Path.Combine(adjustrixADfolder, "Appearance");
        private static readonly string languageFile = Path.Combine(appearanceFolder, "language.txt");
        private static readonly string themeFile = Path.Combine(appearanceFolder, "theme.txt");
        private static readonly string pythonScriptsFolder = Path.Combine(adjustrixADfolder, "PythonScripts");

        private static SystemFileManagement instance;

        public static SystemFileManagement Singleton => GetSingleton();

        private SystemFileManagement()
        {
            EnsurePFFolderExists();
            Directory.CreateDirectory(appearanceFolder);
            Directory.CreateDirectory(pythonScriptsFolder);
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

        public string LogoPath
        {
            get
            {
                return Path.Combine(adjustrixAppearance, "Logo3.ico");
            }
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

        public string PythonScriptsFolder
        {
            get
            {
                return pythonScriptsFolder;
            }
        }

        public string PythonExecutable
        {
            get
            {
                return pythonExecutable;
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
            if (!Directory.Exists(adjustrixPFfolder) ||
                !Directory.Exists(adjustrixDesktop))
            {//todo: think of a good message to display to the user in that case!
                throw new DirectoryNotFoundException($"The system folder {adjustrixPFfolder} was not found!");
            }
        }
    }
}
