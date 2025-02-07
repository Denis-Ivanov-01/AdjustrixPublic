using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AdjustrixBase.Project;
using AdjustrixWPF.Commands;
using AdjustrixWPF.Containers;
using AdjustrixWPF.Enums;
using AdjustrixWPF.View;
using AdjustrixWPF.View.UserControls;
using Forms = System.Windows.Forms;

namespace AdjustrixWPF.ViewModel
{
    public class ProjectFileViewModel : AdjustrixViewModel
    {
        private readonly MessageDelegate messageDelegate;
        private readonly ProjectContainer projectContainer;
        private readonly ProjectFileManager projectFile;
        private readonly LanguageViewModel languageViewModel;

        private bool projectIsNew;
        private bool hasUnsavedChanges;
        private AdjustrixProject project;
        private ProjectType selectedProjectType;
        private ProjectGeneralProperties projectProperties;

        public ObservableCollection<string> ProjectTypes { get; } = EnumHelper.GetEnumStrings<ProjectType>();

        public LanguageViewModel LanguageViewModel => languageViewModel;

        public AdjustrixProject Project
        {
            get => project;
            set
            {
                project = value;
                OnPropertyChanged();
            }
        }

        public string SelectedProjectType
        {
            get => selectedProjectType.ToString();
            set
            {
                selectedProjectType = Enum.Parse<ProjectType>(value);
                OnPropertyChanged();
            }
        }

        public bool HasUnsavedChanges
        {
            get => hasUnsavedChanges;
            set
            {
                hasUnsavedChanges = value;
                OnPropertyChanged();
            }
        }

        public ProjectGeneralProperties ProjectProperties
        { // maybe can be removed - for later
            get => projectProperties;
            set
            {
                projectProperties = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenProject { get; }
        public ICommand SaveProject { get; }
        public ICommand CreateProject { get; }
        public ICommand EditProject { get; }

        public ProjectFileViewModel(ProjectContainer projectStore,
            MessageDelegate messageDelegate)
        {
            this.projectContainer = projectStore;
            this.messageDelegate = messageDelegate;
            languageViewModel = LanguageViewModel.Singleton;
            projectFile = new ProjectFileManager();

            OpenProject = new RelayCommand(OpenProjectFile, _ => true);
            SaveProject = new RelayCommand(SaveProjectFile, _ => CanSaveProject());
            CreateProject = new RelayCommand(CreateNewProject, _ => true);
            EditProject = new RelayCommand(EditProjectSettings, _ => CanEditProjectSettings());

            projectStore.ProjectChanged += OnProjectChanged;
            projectStore.ProjectChangesChanged += OnProjectChangesChanged;

            if (!string.IsNullOrWhiteSpace(App.InitializeFilePath))
            {
                LoadProjectFromFile(App.InitializeFilePath);
                NotifyProjectOpened();
            }
        }

        public bool HandleUnsavedChanges()
        {
            if (HasUnsavedChanges && ConfirmSaveChanges())
            {
                SaveProjectFile(null);
            }

            return true;
        }

        private void OnProjectChangesChanged(bool value) => HasUnsavedChanges = value;

        private void OnProjectChanged(AdjustrixProject project) => Project = project;

        private void CreateNewProject(object parameter)
        {
            if (!HandleUnsavedChanges())
            {
                return;
            }

            string selectedPath = ShowFolderDialog();
            if (string.IsNullOrWhiteSpace(selectedPath))
            {
                return;
            }

            if (CreateNewProjectFile(selectedPath))
            {
                NotifyProjectCreated();
            }
        }

        private string ShowFolderDialog()
        {
            using Forms.FolderBrowserDialog folderBrowser = new()
            {
                Description = languageViewModel.ChooseProjectFolder,
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true
            };

            return folderBrowser.ShowDialog() == Forms.DialogResult.OK ? folderBrowser.SelectedPath : string.Empty;
        }

        private bool CreateNewProjectFile(string folderPath)
        {
            projectIsNew = true;
            ProjectProperties = new();

            var dialog = new CreateProjectPrompt(this);
            dialog.ShowDialog();

            if (!dialog.Confirmed)
            {
                return false;
            }

            project = ProjectFactory.CreateProject(ProjectProperties, selectedProjectType);
            SaveProjectToFile(folderPath);
            UpdateProjectProperties();
            projectContainer.ChangeProject(project);
            return true;
        }

        private void OpenProjectFile(object parameter)
        {
            if (!HandleUnsavedChanges())
            {
                return;
            }

            var dialog = new Forms.OpenFileDialog
            {
                Filter = "Adjustrix project file (.adjx)|*.adjx",
                DefaultExt = ".adjx"
            };

            if (dialog.ShowDialog() == Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
            {
                LoadProjectFromFile(dialog.FileName);
                NotifyProjectOpened();
            }
        }

        private void SaveProjectFile(object parameter)
        {
            SaveProjectToFile(projectFile.LastProjectFolder);
            NotifyProjectSaved();
        }

        private void EditProjectSettings(object parameter)
        {
            var initialProps = ProjectProperties.GetState();

            var dialog = new EditProjectPrompt(this);
            dialog.ShowDialog();

            if (dialog.ApplyEdits)
            {
                ApplyProjectProperties();
                projectContainer.ChangeProject(project);
            }
            else
            {
                ProjectProperties = initialProps;
            }
        }

        private bool CanSaveProject() => project != null;

        private bool CanEditProjectSettings() => project != null;

        private bool ConfirmSaveChanges()
        {
            YesNoPrompt prompt = new(this, LanguageViewModel.SaveProjectQuestion);
            prompt.ShowDialog();
            return prompt.Result == YesNoPromptResult.Yes;
        }

        private void SaveProjectToFile(string folderPath)
        {
            projectFile.ToFile(project, projectIsNew, folderPath);
            projectContainer.SetChanges(false);
            projectContainer.ChangeProjectFolder(folderPath);
        }

        private void LoadProjectFromFile(string filePath)
        {
            project = projectFile.FromFile(filePath);
            projectContainer.ChangeProject(project);
            projectContainer.SetChanges(false);
            projectContainer.ChangeProjectFolder(projectFile.LastProjectFolder);
            UpdateProjectProperties();
        }

        private void UpdateProjectProperties()
        {
            if (project == null)
            {
                return;
            }

            ProjectProperties ??= new(
                project.Name,
                project.SiteName,
                project.Contractor,
                project.Client);
        }

        private void ApplyProjectProperties()
        {
            if (project == null || ProjectProperties == null)
            {
                return;
            }

            project.Name = ProjectProperties.ProjectName;
            project.SiteName = ProjectProperties.SiteName;
            project.Contractor = ProjectProperties.Contractor;
            project.Client = ProjectProperties.Client;
        }

        private void NotifyProjectCreated()
        {
            var message = string.Format(
                languageViewModel.ProjectCreatedMessagePattern,
                project.Name,
                projectFile.LastProjectFolder);

            messageDelegate.ChangeMessage(message);
        }

        private void NotifyProjectOpened()
        {
            var message = string.Format(
                languageViewModel.ProjectOpenedMessagePattern,
                project.Name);

            messageDelegate.ChangeMessage(message);
        }

        private void NotifyProjectSaved()
        {
            var message = string.Format(
                languageViewModel.ProjectSavedMessagePattern,
                project.Name,
                projectFile.LastProjectFolder);

            messageDelegate.ChangeMessage(message, TimeSpan.FromSeconds(5));
        }
    }
}
