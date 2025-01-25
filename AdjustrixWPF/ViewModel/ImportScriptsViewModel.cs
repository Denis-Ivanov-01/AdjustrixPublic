using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using AdjustrixWPF.Model;
using AdjustrixWPF.View.UserControls;
using AdjustrixWPF.View.UserControls.Composite;
using Microsoft.Win32;
using Adjustment.Project;
using System.Windows;
using System;
using System.Threading;

namespace AdjustrixWPF.ViewModel
{
    public class ImportScriptsViewModel : AdjustrixViewModel
    {
        private readonly PythonScriptFileManager scriptFileManager;
        private readonly ProjectContainer projectContainer;
        private AdjustrixProject currentProject;
        private ProjectType currentProjectType;

        private MessageDelegate messageDelegate;

        private ScriptResultContainer scriptResultContainer;
        private PythonProcessManager scriptProcessManager;

        private PythonScriptsWindow scriptsWindow;

        private ObservableCollection<CustomImportScript> importScripts = new();
       
		public ObservableCollection<CustomImportScript> ImportScripts
		{
			get { return importScripts; }
			set { importScripts = value; }
		}

        private ObservableCollection<string> inputParameterTypes = EnumHelper.GetEnumStrings<ScriptParameterType>();

        public ObservableCollection<string> InputParameterTypes
        {
            get { return inputParameterTypes; }
            private set { inputParameterTypes = value; }
        }

        private ScriptParameterType selectedParameterType;

        public ScriptParameterType SelectedParameterType
        {
            get { return selectedParameterType; }
            set
            {
                selectedParameterType = value;
            }
        }

        private CustomImportScript newImportScript;

        public CustomImportScript NewImportScript
        {
            get { return newImportScript; }
            set { newImportScript = value; }
        }

        public ICommand OpenScriptsList { get; }
        public ICommand RegisterScript { get; }
        public ICommand OpenScriptFileDialog { get; }
        public ICommand RunScript { get; }
        public ICommand DeleteScript { get; }

        public ImportScriptsViewModel(ProjectContainer projectContainer, MessageDelegate messageDelegate)
        {
            this.messageDelegate = messageDelegate;

            scriptFileManager = new(SystemFileManagement.Singleton.PythonScriptsFolder);
            this.projectContainer = projectContainer;
            this.projectContainer.ProjectChanged += OnProjectChanged;
            this.projectContainer.ProjectTypeChanged += OnProjectTypeChanged;

            scriptResultContainer = new();
            scriptProcessManager = new(scriptResultContainer);
            scriptResultContainer.ResultChanged += OnResultChanged;

            ScanImportScripts();
            OpenScriptsList = new RelayCommand(OpenScriptsWindow);
            RegisterScript = new RelayCommand(RegisterScriptFile);
            OpenScriptFileDialog = new RelayCommand(OpenScriptDialog);
            DeleteScript = new RelayCommand(RemoveScript);
            RunScript = new RelayCommand(ExecuteScript, CanExecuteScript);
        }

        private void OnProjectTypeChanged(ProjectType type)
        {
            currentProjectType = type;
        }

        private void OnResultChanged(ScriptResult result)
        {
            Application.Current.Dispatcher.Invoke((Delegate)(() =>
            {
                if (result.ExitCode == 0)
                {
                    LevelingProject proj = (LevelingProject)currentProject;
                    proj.KnownBenchmarks = result.Data.KnownBenchmarks;
                    proj.HeightDifferences = result.Data.HeightDifferences;
                    projectContainer.ChangeProject(proj);
                    string message = string.Format(LanguageViewModel.DataLoadedMessagePattern, 
                        result.Data.HeightDifferences.Count, 
                        result.Data.KnownBenchmarks.Count);
                    messageDelegate.ChangeMessage(message);
                }
                else
                {
                    ProcessingErrorPrompt prompt = new(this, result.Error);
                    messageDelegate.ChangeMessage("");
                }
            }));
        }

        private void OnProjectChanged(AdjustrixProject project)
        {
            currentProject = project;
        }

        private void OpenScriptsWindow(object parameter)
        {
            PythonScriptsWindow window = new PythonScriptsWindow();
            scriptsWindow = window;
            window.DataContext = this;
            window.ShowDialog();
        }

        private bool CanExecuteScript(object parameter)
        {
            return currentProject != null;
        }

        private void ExecuteScript(object parameter)
        {
            if (HandleExistingData())
            {
                CustomImportScript script = (CustomImportScript)parameter;
                NotifyScriptExecuting(script);
                scriptsWindow.Close();
                scriptProcessManager.ExecuteScript(script);
            }
        }
        private void NotifyScriptExecuting(CustomImportScript script)
        {
            string message = string.Format(LanguageViewModel.ExecutingScriptPattern, script.Name);
            messageDelegate.ChangeMessage(message, Timeout.InfiniteTimeSpan);
        }

        private void RemoveScript(object parameter)
        {
            CustomImportScript script = (CustomImportScript)parameter;
            scriptFileManager.DeleteScript(script.Name);
            ImportScripts.Remove(script);
        }

        private void RegisterScriptFile(object parameter)
        {
            NewImportScript = new();
            AddImportScriptPrompt prompt = new(this);
            prompt.ShowDialog();
            if (prompt.RegisterScript && ValidateNewScript())
            {
                NewImportScript.ScriptParameterType = SelectedParameterType;
                scriptFileManager.CreateScriptFolder(NewImportScript);
                ImportScripts.Add(NewImportScript);
            }
        }

        private bool ValidateNewScript()
        {
            return !string.IsNullOrWhiteSpace(NewImportScript.Name) && 
                !string.IsNullOrWhiteSpace(NewImportScript.FileExtension) &&
                FileFilterValidator.IsValidFileFilter(NewImportScript.FileFilter) &&
                FileFilterValidator.IsValidFileExtension(NewImportScript.FileExtension);
        }

        private void OpenScriptDialog(object parameter)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Python script file (.py)|*.py",
                DefaultExt = ".py"
            };
            bool? result = dialog.ShowDialog();
            if (result == true && !string.IsNullOrWhiteSpace(dialog.FileName))
            {
                NewImportScript.ScriptPath = dialog.FileName;
            }
        }

        private void ScanImportScripts()
        {
            foreach (CustomImportScript script in scriptFileManager.ScanForScripts())
            {
                ImportScripts.Add(script);
            }
        }

        private bool HandleExistingData()
        {
            switch (currentProjectType)
            {
                case ProjectType.Leveling:
                    LevelingProject levelingProject = (LevelingProject)currentProject;
                    if (levelingProject.KnownBenchmarks.Count > 0 || levelingProject.HeightDifferences.Count > 0)
                    {
                        return PromptOverwrite();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return true;
        }

        private bool PromptOverwrite()
        {
            OverwriteDataPrompt prompt = new(this);
            prompt.ShowDialog();
            return prompt.OverwriteData;
        }
    }

    public class FileFilterValidator
    {
        /// <summary>
        /// Validates a file filter string in the format: "Description (.ext)|*.ext".
        /// </summary>
        /// <param name="filter">The file filter string to validate.</param>
        /// <returns>True if the filter string is valid, otherwise false.</returns>
        public static bool IsValidFileFilter(string filter)
        {
            // Regex to validate file filter format
            string pattern = @"^[\w\s]+\s\(\.[a-zA-Z0-9]+\)\|\*\.[a-zA-Z0-9]+$";
            return Regex.IsMatch(filter, pattern);
        }

        /// <summary>
        /// Validates a file extension string like ".py".
        /// </summary>
        /// <param name="extension">The file extension to validate.</param>
        /// <returns>True if the extension string is valid, otherwise false.</returns>
        public static bool IsValidFileExtension(string extension)
        {
            // Regex to validate a single file extension like ".py"
            string pattern = @"^\.[a-zA-Z0-9]+$";
            return Regex.IsMatch(extension, pattern);
        }
    }
}
