using System;
using System.Collections.Generic;
using System.IO;
using AdjustrixWPF.ViewModel;

namespace AdjustrixWPF.Model
{
    public class PythonScriptFileManager
    {

        private const string scriptFileName = "script.py";
        private const string filterFileName = "filter.txt";
        private const string extensionFileName = "extension.txt";
        private const string parameterFileName = "parameter.txt";

        private readonly string pythonScriptsFolder;

        public PythonScriptFileManager(string scriptsFolder)
        {
            pythonScriptsFolder = scriptsFolder;
        }

        public void DeleteScript(string name)
        {
            string path = Path.Combine(pythonScriptsFolder, name);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public List<CustomImportScript> ScanForScripts()
        {
            List<CustomImportScript> scripts = new();
            foreach (string dir in Directory.EnumerateDirectories(pythonScriptsFolder))
            {
                if (TryParseScript(dir, out CustomImportScript script))
                {
                    scripts.Add(script);
                }
            }
            return scripts;
        }

        private bool TryParseScript(string scriptDir, out CustomImportScript script)
        {
            script = new();

            string extensionFilePath = GetExtensionFilePath(scriptDir);
            if (!File.Exists(extensionFilePath))
            {
                return false;
            }

            string extension = File.ReadAllText(extensionFilePath);
            if (!FileFilterValidator.IsValidFileExtension(extension))
            {
                return false;
            }

            script.FileExtension = extension;

            string filterFilePath = GetFilterFilePath(scriptDir);
            if (!File.Exists(filterFilePath))
            {
                return false;
            }

            string filter = File.ReadAllText(filterFilePath);
            if (!FileFilterValidator.IsValidFileFilter(filter))
            {
                return false;
            }

            script.FileFilter = filter;

            script.Name = Path.GetFileName(scriptDir)!;

            string scriptFile = GetScriptFilePath(scriptDir);
            if (!File.Exists(scriptFile))
            {
                return false;
            }

            script.ScriptPath = scriptFile;

            string parameterTypeFile = GetParameterFilePath(scriptDir);
            if (!File.Exists(parameterTypeFile))
            {
                return false;
            }

            string paramType = File.ReadAllText(parameterTypeFile);
            if (Enum.TryParse(typeof(ScriptParameterType), paramType, out object result))
            {
                script.ScriptParameterType = (ScriptParameterType)result;
            }
            else
            {
                return false;
            }
            return true;
        }

        public void RegisterScript(CustomImportScript script)
        {
            string scriptFolderPath = Path.Combine(pythonScriptsFolder, script.Name);
            if (Directory.Exists(scriptFolderPath))
            {
                Directory.Delete(scriptFolderPath, true);
            }
            Directory.CreateDirectory(scriptFolderPath);

            string scriptPath = GetScriptFilePath(scriptFolderPath);
            File.Copy(script.ScriptPath, scriptPath);

            string filterFilePath = GetFilterFilePath(scriptFolderPath);
            File.WriteAllText(filterFilePath, script.FileFilter);

            string extensionFilePath = GetExtensionFilePath(scriptFolderPath);
            File.WriteAllText(extensionFilePath, script.FileExtension);

            string parameterTypeFilePath = GetParameterFilePath(scriptFolderPath);
            File.WriteAllText(parameterTypeFilePath, script.ScriptParameterType.ToString());
        }

        private static string GetParameterFilePath(string scriptPath)
        {
            return Path.Combine(scriptPath, parameterFileName);
        }

        private static string GetScriptFilePath(string scriptFolderPath)
        {
            return Path.Combine(scriptFolderPath, scriptFileName);
        }

        private static string GetExtensionFilePath(string scriptFolderPath)
        {
            return Path.Combine(scriptFolderPath, extensionFileName);
        }

        private static string GetFilterFilePath(string scriptFolderPath)
        {
            return Path.Combine(scriptFolderPath, filterFileName);
        }
    }
}
