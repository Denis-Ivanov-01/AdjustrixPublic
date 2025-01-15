using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Adjustment.Project;
using AdjustrixWPF.View.UserControls;
using Forms = System.Windows.Forms;

namespace AdjustrixWPF.ViewModel
{
    public class ProjectViewModel : ViewModelBase
    {
        private ProjectStore projectStore;
        private bool projectLoaded = false;
        private bool projectChanged = false;
        private bool projectSaved = true;
        private AdjustrixProject project;
        private readonly ProjectFileManager projectFile;
        private readonly ObservableCollection<string> projectTypes = EnumHelper.GetEnumStrings<ProjectType>();
        private ProjectType selectedProjectType;
        private ProjectCreationProperties projectProperties;

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
                projectStore.ChangeProject(value);
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

        public ProjectCreationProperties ProjectProperties
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

        public ProjectViewModel(ProjectStore projectStore)
        {
            this.projectStore = projectStore;
            languageViewModel = LanguageViewModel.Singleton;
            projectFile = new ProjectFileManager();
            OpenProject = new RelayCommand(OpenProjectFile, CanOpenProject);
            SaveProject = new RelayCommand(SaveProjectFile, CanSaveProject);
            CreateProject = new RelayCommand(CreateNewProject, CanCreateProject);
            projectStore.ProjectChanged += OnProjectChanged;
        }

        private void OnProjectChanged(AdjustrixProject project)
        {
            
        }

        public bool ProjectLoaded
        {
            get { return projectLoaded; }
            set
            {
                projectLoaded = value;
                OnPropertyChanged();
            }
        }

        public bool ProjectChanged
        {
            get { return projectChanged; }
            set
            {
                projectChanged = value;
                OnPropertyChanged();
            }
        }

        public bool ProjectSaved
        {
            get { return projectSaved; }
            set
            {
                projectSaved = value;
                OnPropertyChanged();
            }
        }

        private void CreateNewProject(object parameter)
        {
            if (!projectSaved && AskProjectClose())
            { // The user has unsaved changes and wants to close the current project
                if (AskProjectSave())
                { // Saving the changes
                    projectFile.ToFile(project);
                }
            }

            using (Forms.FolderBrowserDialog folderBrowser = new())
            {
                folderBrowser.Description = LanguageViewModel.ChooseProjectFolder;
                folderBrowser.UseDescriptionForTitle = true;
                folderBrowser.ShowNewFolderButton = true;

                Forms.DialogResult result = folderBrowser.ShowDialog();

                if (result == Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
                {
                    ProjectProperties = new();
                    CreateProjectPrompt dialog = new(this);
                    dialog.ShowDialog();
                    if (!dialog.Confirmed) { return; }
                    project = ProjectFactory.CreateProject(ProjectProperties, selectedProjectType);
                    //todo: Check if file exists before. Prompt to overwrite it
                    projectFile.ToFile(project, folderBrowser.SelectedPath);
                    ProjectSaved = true;
                    ProjectLoaded = true;
                    ProjectChanged = false;
                }
            }
        }

        public bool CanCreateProject(object parameter)
        {
            return true;
        }

        private void OpenProjectFile(object parameter)
        {
            var dialog = new Forms.OpenFileDialog();
            dialog.Filter = "Adjustrix project file (.adjx)|*.adjx";
            dialog.DefaultExt = ".adjx";

            Forms.DialogResult result = dialog.ShowDialog();
            if (result == Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
            {
                project = projectFile.FromFile(dialog.FileName);
                ProjectLoaded = true;
                ProjectSaved = true;
                ProjectChanged = false;
                projectStore.ChangeProject(project);
            }
        }

        private bool CanOpenProject(object parameter)
        {
            return true;
        }

        private void SaveProjectFile(object parameter)
        {
            if (projectChanged)
            {
                bool result = AskProjectSave();
                if (result == false) { return; }
            }

            projectFile.ToFile(project);
            ProjectSaved = true;
            ProjectChanged = false;
        }

        private bool CanSaveProject(object parameter)
        {
            return project != null;
        }

        private bool AskProjectSave()
        {
            SaveProjectPrompt dialog = new(this);
            dialog.ShowDialog();
            return dialog.SaveProject;
        }

        private bool AskProjectClose()
        {
            CloseProjectPrompt dialog = new(this);
            dialog.ShowDialog();
            return dialog.CloseProject;
        }
    }
}
