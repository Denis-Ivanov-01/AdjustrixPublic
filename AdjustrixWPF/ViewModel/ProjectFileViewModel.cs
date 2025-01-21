using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Adjustment.Project;
using AdjustrixWPF.View;
using AdjustrixWPF.View.UserControls;
using Forms = System.Windows.Forms;

namespace AdjustrixWPF.ViewModel
{
    public class ProjectFileViewModel : ViewModelBase
    {

        private readonly MessageDelegate messageDelegate;

        private readonly ProjectContainer projectContainer;
        private bool projectIsNew;
        private bool hasUnsavedChanges;
        private AdjustrixProject project;
        private readonly ProjectFileManager projectFile;
        private readonly ObservableCollection<string> projectTypes = EnumHelper.GetEnumStrings<ProjectType>();
        private ProjectType selectedProjectType;
        private ProjectGeneralProperties projectProperties;

        //private float saveProjectOpacity = 0.0f;

        private readonly LanguageViewModel languageViewModel;

        public LanguageViewModel LanguageViewModel
        {
            get
            {
                return languageViewModel;
            }
        }

        public AdjustrixProject Project
        {
            get { return project; }
            set
            {
                //projectStore.ChangeProject(value);
                project = value; //todo: remove?
                OnPropertyChanged();
            }
        }


        public ObservableCollection<string> ProjectTypes
        {
            get { return projectTypes; }
        }

        public string SelectedProjectType
        {
            get
            {
                return selectedProjectType.ToString();
            }
            set
            {
                selectedProjectType = (ProjectType)Enum.Parse(typeof(ProjectType), value);
                OnPropertyChanged();
            }
        }

        public bool HasUnsavedChanges
        {
            get
            {
                return hasUnsavedChanges;
            }
            set
            {
                hasUnsavedChanges = value;
                OnPropertyChanged();
            }
        }

        public ProjectGeneralProperties ProjectProperties
        {
            get { return projectProperties; }
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

        public ProjectFileViewModel(ProjectContainer projectStore, MessageDelegate messageDelegate)
        {
            this.projectContainer = projectStore;
            this.messageDelegate = messageDelegate;
            hasUnsavedChanges = false;
            languageViewModel = LanguageViewModel.Singleton;
            projectFile = new ProjectFileManager();
            OpenProject = new RelayCommand(OpenProjectFile, CanOpenProject);
            SaveProject = new RelayCommand(SaveProjectFile, CanSaveProject);
            CreateProject = new RelayCommand(CreateNewProject, CanCreateProject);
            EditProject = new RelayCommand(EditProjectSettings, CanEditProjectSettings);
            projectStore.ProjectChanged += OnProjectChanged;
            projectStore.ProjectChangesChanged += OnProjectChangesChanged;
        }

        private void OnProjectChangesChanged(bool value)
        {
            HasUnsavedChanges = value;
        }

        private void OnProjectChanged(AdjustrixProject project)
        {
            Project = project;
        }

        private void CreateNewProject(object parameter)
        {
            PromptForUnsavedChangesAndSave();

            string selectedPath = ShowFolderDialog();
            if (string.IsNullOrWhiteSpace(selectedPath))
            {
                return;
            }

            CreateNewProjectFile(selectedPath);
            messageDelegate.ChangeMessage($"Created new project: {project.Name} in folder {projectFile.LastProjectFolder}", TimeSpan.FromSeconds(3));
        }

        private string ShowFolderDialog()
        {
            using (Forms.FolderBrowserDialog folderBrowser = new())
            {
                folderBrowser.Description = LanguageViewModel.ChooseProjectFolder;
                folderBrowser.UseDescriptionForTitle = true;
                folderBrowser.ShowNewFolderButton = true;

                if (folderBrowser.ShowDialog() == Forms.DialogResult.OK)
                {
                    return folderBrowser.SelectedPath;
                }

                return string.Empty;
            }
        }

        private void CreateNewProjectFile(string folderPath)
        {
            projectIsNew = true;
            ProjectProperties = new();
            CreateProjectPrompt dialog = new(this);
            dialog.ShowDialog();

            if (!dialog.Confirmed)
            {
                return;
            }

            project = ProjectFactory.CreateProject(ProjectProperties, selectedProjectType);
            projectFile.ToFile(project, projectIsNew, folderPath);
            projectContainer.ChangeProject(project);
            projectContainer.ChangeProjectFolder(projectFile.LastProjectFolder);
            UpdateProjectProperties();
        }

        public bool CanCreateProject(object parameter)
        {
            return true;
        }

        private void OpenProjectFile(object parameter)
        {
            projectIsNew = false;
            PromptForUnsavedChangesAndSave();

            var dialog = new Forms.OpenFileDialog();
            dialog.Filter = "Adjustrix project file (.adjx)|*.adjx";
            dialog.DefaultExt = ".adjx";

            Forms.DialogResult result = dialog.ShowDialog();
            if (result == Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
            {
                project = projectFile.FromFile(dialog.FileName);
                projectContainer.ChangeProject(project);
                projectContainer.SetChanges(false);
                projectContainer.ChangeProjectFolder(projectFile.LastProjectFolder);
                UpdateProjectProperties();
                messageDelegate.ChangeMessage($"Opened project {project.Name}", TimeSpan.FromSeconds(3));
            }
        }

        private bool CanOpenProject(object parameter)
        {
            return true;
        }

        private void SaveProjectFile(object parameter)
        {
            projectFile.ToFile(project, projectIsNew);
            projectContainer.SetChanges(false);
            messageDelegate.ChangeMessage($"Saved project {project.Name} in {projectFile.LastProjectFolder}", TimeSpan.FromSeconds(5));
        }

        private bool CanSaveProject(object parameter)
        {
            return project != null;
        }

        private void EditProjectSettings(object param)
        {
            ProjectGeneralProperties initialProps = ProjectProperties.GetState();
            EditProjectPrompt prompt = new(this);
            prompt.ShowDialog();
            if (prompt.ApplyEdits)
            {
                initialProps = null;
                ApplyProjectProperties();
                projectContainer.ChangeProject(project);
            }
            else
            {
                ProjectProperties = initialProps;
            }
        }

        private bool CanEditProjectSettings(object param)
        {
            return project != null;
        }

        public void PromptForUnsavedChangesAndSave()
        {
            if (HasUnsavedChanges && AskProjectSave())
            {
                projectFile.ToFile(project, projectIsNew);
            }
        }

        private bool AskProjectSave()
        {
            SaveProjectPrompt dialog = new(this);
            dialog.ShowDialog();
            return dialog.SaveProject;
        }

        private void UpdateProjectProperties()
        {
            if (project == null) { return; }
            ProjectProperties ??= new(project.Name, project.SiteName, project.Contractor, project.Client);
        }

        private void ApplyProjectProperties()
        {
            if (project != null && ProjectProperties != null)
            {
                project.Client = ProjectProperties.Client;
                project.Contractor = ProjectProperties.Contractor;
                project.SiteName = ProjectProperties.SiteName;
                project.Name = ProjectProperties.ProjectName;
            }
        }

        private void OnProjectChanges()
        {

        }

        private bool AskProjectClose()
        {
            CloseProjectPrompt dialog = new(this);
            dialog.ShowDialog();
            return dialog.CloseProject;
        }
    }
}
