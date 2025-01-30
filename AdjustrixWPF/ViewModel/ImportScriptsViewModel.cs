using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using AdjustrixBase.Project;
using AdjustrixBase.DataModels;
using AdjustrixBase.NetworkAnalysis;
using AdjustrixWPF.View.UserControls;
using AdjustrixWPF.View.UserControls.Composite;
using Microsoft.Win32;
using AdjustrixWPF.Containers;
using AdjustrixWPF.PythonScripting;
using AdjustrixWPF.SystemManagement;
using AdjustrixWPF.Enums;
using AdjustrixWPF.Commands;

namespace AdjustrixWPF.ViewModel
{
    public class ImportScriptsViewModel : AdjustrixViewModel, INotifyDataErrorInfo
    {
        private readonly PythonScriptFileManager scriptFileManager;
        private readonly ProjectContainer projectContainer;
        private AdjustrixProject currentProject;
        private ProjectType currentProjectType;

        private readonly MessageDelegate messageDelegate;

        private readonly ScriptResultContainer scriptResultContainer;
        private readonly PythonProcessManager scriptProcessManager;

        private PythonScriptsWindow scriptsWindow;

        private readonly Dictionary<string, List<string>> _errors = new();

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
                NewImportScript.ScriptParameterType = value;

                // Re-validate FileFilter and FileExtension when type changes
                ValidateProperty(nameof(FileFilter));
                ValidateProperty(nameof(FileExtension));
            }
        }

        public string FileFilter
        {
            get { return NewImportScript.FileFilter; }
            set
            {
                NewImportScript.FileFilter = value;
                ValidateProperty();
            }
        }

        public string FileExtension
        {
            get
            {
                return NewImportScript.FileExtension;
            }
            set
            {
                NewImportScript.FileExtension = value;
                ValidateProperty();
            }
        }

        public string ScriptName
        {
            get
            {
                return NewImportScript.Name;
            }
            set
            {
                NewImportScript.Name = value;
                ValidateProperty();
            }
        }

        public string ScriptPath
        {
            get
            {
                return NewImportScript.ScriptPath;
            }
            set
            {
                NewImportScript.ScriptPath = value;
                ValidateProperty();
            }
        }

        private CustomImportScript newImportScript;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public CustomImportScript NewImportScript
        {
            get { return newImportScript; }
            set 
            {
                newImportScript = value;
                ValidateProperty();
                ValidateProperty(nameof(ScriptPath));
                ValidateProperty(nameof(ScriptName));
                ValidateProperty(nameof(FileExtension));
                ValidateProperty(nameof(FileFilter));
            }
        }

        public ICommand OpenScriptsList { get; }
        public ICommand RegisterScript { get; }
        public ICommand OpenScriptFileDialog { get; }
        public ICommand RunScript { get; }
        public ICommand DeleteScript { get; }

        public bool HasErrors => _errors.Any();

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
                    //There are validations in the graph constructor
                    Graph<HeightDelta> graph = new(result.Data.HeightDifferences);

                    LevelingProject proj = (LevelingProject)currentProject;
                    proj.KnownBenchmarks = result.Data.KnownBenchmarks;
                    proj.HeightDifferences = result.Data.HeightDifferences;
                    proj.EnsureValidDataTypes();
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
            YesNoPrompt prompt = new(this, LanguageViewModel.DeleteScriptQuestion);
            prompt.ShowDialog();
            if (prompt.Result == YesNoPromptResult.Yes)
            {
                CustomImportScript script = (CustomImportScript)parameter;
                scriptFileManager.DeleteScript(script.Name);
                ImportScripts.Remove(script);
            }
        }

        private void RegisterScriptFile(object parameter)
        {
            YesNoPrompt yesNo = new(this, LanguageViewModel.TrustScriptAuthorQuestion);
            yesNo.ShowDialog();
            if (yesNo.Result != YesNoPromptResult.Yes)
            {
                return;
            }
            NewImportScript = new()
            {
                ScriptParameterType = SelectedParameterType
            };
            AddImportScriptPrompt prompt = new(this);
            prompt.ShowDialog();
            if (prompt.RegisterScript && ValidateNewScript())
            {
                if (!PromptNullScriptPath()) return;
                scriptFileManager.RegisterScript(NewImportScript);
                ImportScripts.Add(NewImportScript);
            }
        }

        private bool ValidateNewScript()
        {
            if (NewImportScript.ScriptParameterType == ScriptParameterType.Directory)
            {
                return !string.IsNullOrWhiteSpace(NewImportScript.Name);
            }
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

        private bool PromptNullScriptPath()
        {
            if (string.IsNullOrWhiteSpace(ScriptPath))
            {
                ProcessingErrorPrompt prompt = new(this, LanguageViewModel.MustSpecifyScript);
                prompt.ShowDialog();
                return false;
            }
            return true;
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName == nameof(FileFilter) || propertyName == nameof(FileExtension))
            {
                Console.WriteLine();
            }
            return _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;
        }

        private void AddError(string propertyName, string errorMessage)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }

            _errors[propertyName].Add(errorMessage);
        }

        private void ValidateProperty([CallerMemberName] string propName = "")
        {
            if (_errors.ContainsKey(propName))
            {
                _errors.Remove(propName);
            }

            switch (propName)
            {
                case nameof(FileFilter):
                case nameof(FileExtension):
                    ValidateFilterExtension();
                    break;

                case nameof(ScriptName):
                    ValidateScriptName();
                    break;
                case nameof(ScriptPath):
                    ValidateScriptPath();
                    break;
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propName));
        }

        private void ValidateFilterExtension()
        {
            if (NewImportScript.ScriptParameterType == ScriptParameterType.FilePath)
            {
                if (!FileFilterValidator.IsValidFileExtension(NewImportScript.FileExtension))
                {
                    AddError(nameof(FileExtension), LanguageViewModel.MustEnterValidExtensionMessage);
                }
                if (!FileFilterValidator.IsValidFileFilter(NewImportScript.FileFilter))
                {
                    AddError(nameof(FileFilter), LanguageViewModel.MustEnterValidFilterMessage);
                }
            }
        }

        private void ValidateScriptName()
        {
            if (string.IsNullOrWhiteSpace(ScriptName))
            {
                AddError(nameof(ScriptName), LanguageViewModel.MustEnterName);
            }
        }

        private void ValidateScriptPath()
        {
            if (string.IsNullOrWhiteSpace(ScriptPath) || !ScriptPath.EndsWith(".py"))
            {
                AddError(nameof(ScriptPath), LanguageViewModel.MustSpecifyScript);
            }
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
            if (string.IsNullOrWhiteSpace(filter)) return false;

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
            if (string.IsNullOrWhiteSpace(extension)) return false;

            // Regex to validate a single file extension like ".py"
            string pattern = @"^\.[a-zA-Z0-9]+$";
            return Regex.IsMatch(extension, pattern);
        }
    }
}
