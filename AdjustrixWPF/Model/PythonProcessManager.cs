using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Adjustment.Adjustment;
using AdjustrixWPF.ViewModel;

namespace AdjustrixWPF.Model
{
    public class PythonProcessManager
    {
        private ScriptResultContainer resultContainer;

        private readonly string pythonExecutable = SystemFileManagement.Singleton.PythonExecutable;
        public bool ProcessSuccessful { get; private set; } = false;

        public PythonProcessManager(ScriptResultContainer container)
        {
            resultContainer = container;
        }

        public void ExecuteScript(CustomImportScript script)
        {
            if (script.ScriptParameterType == ScriptParameterType.FilePath)
            {
                if (GetFileScriptArgs(script, out string args))
                {
                    ExecuteScript(args, script);
                }
            }
            else
            {
                if (GetFolderScriptArgs(script, out string args))
                {
                    ExecuteScript(args, script);
                }
            }
        }

        private bool GetFileScriptArgs(CustomImportScript script, out string args)
        {
            OpenFileDialog dialog = new();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
            {
                string scriptPath = $"\"{script.ScriptPath}\"";
                string filePath = $"\"{dialog.FileName}\"";
                args = string.Join(" ", scriptPath, filePath);
                return true;
            }
            args = "";
            return false;
        }

        private bool GetFolderScriptArgs(CustomImportScript script, out string args)
        {
            FolderBrowserDialog dialog = new();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                string scriptPath = $"\"{script.ScriptPath}\"";
                string selectedPath = $"\"{dialog.SelectedPath}\"";
                args = string.Join(" ", scriptPath, selectedPath);
                return true;
            }
            args = "";
            return false;
        }

        private void ExecuteScript(string arguments, CustomImportScript script)
        {
            ProcessStartInfo info = new();
            info.Arguments = arguments;
            info.FileName = pythonExecutable;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            Task.Run(async () =>
            {
                Process process = new();
                process.StartInfo = info;
                process.Start();

                string error = await process.StandardError.ReadToEndAsync();
                string output = await process.StandardOutput.ReadToEndAsync();

                await process.WaitForExitAsync();
                ScriptResult result = new()
                {
                    ExitCode = process.ExitCode
                };
                if (process.ExitCode == 0)
                {
                    try
                    {
                        JSONLevelingData data = JSONLevelingData.FromString(output);
                        result.Data = data;
                        ProcessSuccessful = true;
                    }
                    catch (Exception ex)
                    {
                        result.Error = ex.Message;
                    }
                }
                else if (process.ExitCode == 1)
                {
                    result.Error = error;
                }
                else
                {//exit code 2
                    result.Error = "Severe error with the Python script. Make sure to follow the guide.";
                }

                resultContainer.ChangeScriptResult(result);
            });
        }
    }

    public class ScriptResult
    {
        public int ExitCode { get; set; }

        public string Error { get; set; }

        public JSONLevelingData Data { get; set; }
    }
}
