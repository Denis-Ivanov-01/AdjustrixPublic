using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace AdjustrixWPF.ViewModel
{
    internal class StringEntry
    {
        public string Bulgarian { get; set; }
        public string English { get; set; }
        public StringEntry(string bg, string en)
        {
            Bulgarian = bg;
            English = en;
        }

        public string GetString(Language currentLanguage)
        {
            if (currentLanguage == Language.English)
            {
                return English;
            }
            return Bulgarian;
        }
    }

    public class LanguageViewModel : ViewModelBase
    {
        private readonly string[] propertyNames;

        private static LanguageViewModel instance;
        public static LanguageViewModel Singleton => GetInstance();

        private readonly StringEntry file = new("Файл", "File");
        private readonly StringEntry data = new("Данни", "Data");
        private readonly StringEntry appearance = new("Изглед", "Appearance");
        private readonly StringEntry language = new("Език", "Language");
        private readonly StringEntry theme = new("Тема", "Theme");
        private readonly StringEntry newProject = new("Нов Проект", "New Project");
        private readonly StringEntry saveProject = new("Запази Проект", "Save Project");
        private readonly StringEntry openProject = new("Отвори Проект", "Open Project");
        private readonly StringEntry editProject = new("Редактирай проект", "Edit Project");
        private readonly StringEntry saveProjectQuestion = new("Искате ли да запазите промените по текущия проект?",
            "Do you want to save the changes made to current project?");
        private readonly StringEntry yes = new("Да", "Yes");
        private readonly StringEntry no = new("Не", "No");
        private readonly StringEntry chooseProjectFolder = new("Изберете папка за проекта...", "Choose a project folder...");
        private readonly StringEntry closeProjectQuestion = new("Искате ли да затворите текущия проект?", "Close the current project?");
        private readonly StringEntry projectName = new("Име на проект", "Project name");
        private readonly StringEntry siteName = new("Име на обект", "Site name");
        private readonly StringEntry contractor = new("Изпълнител", "Contractor");
        private readonly StringEntry client = new("Възложител", "Client");
        private readonly StringEntry networkType = new("Тип мрежа", "Network type");
        private readonly StringEntry fromPoint = new("Начална Точка", "From Point");
        private readonly StringEntry toPoint = new("Крайна Точка", "To Point");
        private readonly StringEntry length = new("Дължина", "Length");
        private readonly StringEntry value = new("Стойност", "Value");
        private readonly StringEntry number = new("Номер", "Number");
        private readonly StringEntry elevation = new("Височина", "Elevation");
        private readonly StringEntry toggle = new("Включи/Изключи", "Toggle");
        private readonly StringEntry edit = new("Редактирай", "Edit");
        private readonly StringEntry processing = new("Обработка", "Processing");
        private readonly StringEntry measurements = new("Измервания", "Measurements");
        private readonly StringEntry benchmarks = new("Репери", "Benchmarks");
        private readonly StringEntry enabled = new("Включено", "Enabled");
        private readonly StringEntry dataLoading = new("Зареждане на Данни", "Data Loading");
        private readonly StringEntry importExcel = new("Зареждане на Ексел файл", "Load Excel file");
        private readonly StringEntry customImport = new("Скрипт за вмъкване", "Run Import Script");
        private readonly StringEntry overwriteDataQuestion = new("Искате ли да презапишете данните?", "Do you want to overwrite the data?");
        private readonly StringEntry successfulProcessing = new("Успешна обработка", "Successful processing");
        private readonly StringEntry revealQuestion = new("Искате ли да се покажат докладите от обработката?",
            "Do you want to reveal the processing reports?");
        private readonly StringEntry errorOccurred = new("Възникна грешка", "An error occurred");
        private readonly StringEntry editMeasurement = new("Редактиране на измерване", "Edit measurement");
        private readonly StringEntry save = new("Запази", "Save");
        private readonly StringEntry cancel = new("Откажи", "Cancel");
        private readonly StringEntry nonNumericMessage = new("Трябва да въведете числена стойност", "You must enter a numeric value");
        private readonly StringEntry editPoint = new("Редактиране на точка", "Edit point");
        private readonly StringEntry negativeValueMessage = new("Не може да въведете отрицателна стойност", "You cannot enter a negative value");
        private readonly StringEntry projectSettings = new("Настройки на проекта", "Project settings");

        #region ExceptionMessages
        private readonly StringEntry ioException = new("Проблем с отварянето на файла. Затворете файловете преди да ги вмъквате.",
            "An error occurred while opening the file. Close the files before importing.");
        private readonly StringEntry incorrectGeoAnalysis = new("Неуспешен анализ на мрежата. Проверете конфигурацията.", "Unsuccessful network analysis. Check the network configuration.");
        private readonly StringEntry incorrectAdjustmentRes = new("Не е изпълнена крайната проверка при изравнението.", "The network could not be adjusted. It yielded an incorrec result.");
        private readonly StringEntry networkNotConnected = new("Мрежата не е напълно свързана.", "The network is not fully connected.");
        private readonly StringEntry hangingPoint = new("Точка с номер {0} е висяща.", "Point with number {0} is hanging.");
        private readonly StringEntry duplicateMeasurement = new("Дублирано измерване между точки с номера {0} и {1}.", "Duplicate measurement between points with number {0} and {1}.");
        private readonly StringEntry loopingMeasurement = new("Огледални измервания между точки с номера {0} и {1}.", "Mirrored measurements between points with number {0} and {1}.");

        private readonly StringEntry unknownError = new("Възникна неизвестна грешка.", "An unkown exception occurred.");
        #endregion

        private readonly StringEntry dataLoaded = new("Заредени са {0} измервания и {1} репер(а).", "{0} measurements and {1} benchmarks were loaded.");
        private readonly StringEntry preparingData = new("Подготвят се данните за мрежата...", "Preparing the network data...");
        private readonly StringEntry performingGeoAnalysis = new("Извършва се геометричен анализ на мрежата...", "Performing geometric analysis of the network...");
        private readonly StringEntry adjusting = new("Изравнява се мрежата...", "Adjusting the network...");
        private readonly StringEntry creatingReports = new("Създават се докладите...", "Creating the reports...");
        private readonly StringEntry processingTime = new("Процесът приключи за {0} секунди.", "The processing took {0} seconds.");
        private static Language currentLanguage;

        private readonly StringEntry createdProject = new("Създаден е проект с име {0} в папката {1}.", "A project with the name {0} was created in the folder {1}.");
        private readonly StringEntry openedProject = new("Отворен е проектът {0}.", "The project {0} was opened.");
        private readonly StringEntry savedProject = new("Проектът {0} е запазен в папката {1}.", "The project {0} was saved in the folder {1}.");

        private readonly StringEntry importScripts = new("Скриптове за Импорт", "Import Scripts");
        private readonly StringEntry name = new("Име", "Name");
        private readonly StringEntry extension = new("Файлово разширение", "File extension");
        private readonly StringEntry scriptProperteis = new("Настройки на скрипт", "Script properties");
        private readonly StringEntry inputParameterType = new("Вид входен параметър", "Input parameter type");
        private readonly StringEntry filter = new("Филтър", "Filter");
        private readonly StringEntry scriptFile = new("Скрипт файл", "Script file");
        private readonly StringEntry executingScript = new("Изпълнява се {0}", "Running {0}");
        private readonly StringEntry deleteScriptQuestion = new("Искате ли да изтриете скрипта?", "Do you want to delete the script?");
        private readonly StringEntry trustScriptSourceQustion = new("Доверявате ли се на автора на скрипта?", "Do you trust the author of the script?");
        
        private LanguageViewModel()
        {
            propertyNames = GetClassProperties();
            Languages = new();
            Languages.Add("Bulgarian");
            Languages.Add("English");
            currentLanguage = ParseLanguageString(SystemFileManagement.Singleton.LanguageString);
        }

        public static Language SelectedLanguage
        {
            get
            {
                return currentLanguage;
            }
            set
            {
                currentLanguage = value;
            }
        }

        public string CurrentLanguage
        {
            get
            {
                return currentLanguage.ToString();
            }
            set
            {
                currentLanguage = ParseLanguageString(value);
                SystemFileManagement.Singleton.LanguageString = value;
                OnPropertiesChanged();
            }
        }

        public string File
        {
            get { return file.GetString(currentLanguage); }
        }

        public string Data
        {
            get { return data.GetString(currentLanguage); }
        }

        public string Appearance
        {
            get { return appearance.GetString(currentLanguage); }
        }

        public string LanguageString
        {
            get { return language.GetString(currentLanguage); }
        }

        public string ThemeString
        {
            get { return theme.GetString(currentLanguage); }
        }

        public string SelectedTheme
        {
            get
            {
                return ThemeViewModel.SelectedTheme;
            }
            set
            {
                ThemeViewModel.SelectedTheme = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Themes
        {
            get
            {
                return ThemeViewModel.Singleton.Themes;
            }
            set
            {
                ThemeViewModel.Singleton.Themes = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Languages { get; set; }

        public string NewProject
        {
            get
            {
                return newProject.GetString(currentLanguage);
            }
        }

        public string SaveProject
        {
            get
            {
                return saveProject.GetString(currentLanguage);
            }
        }

        public string OpenProject
        {
            get
            {
                return openProject.GetString(currentLanguage);
            }
        }

        public string EditProject
        {
            get
            {
                return editProject.GetString(currentLanguage);
            }
        }

        public string SaveProjectQuestion
        {
            get
            {
                return saveProjectQuestion.GetString(currentLanguage);
            }
        }

        public string Yes
        {
            get
            {
                return yes.GetString(currentLanguage);
            }
        }

        public string No
        {
            get
            {
                return no.GetString(currentLanguage);
            }
        }


        public string ChooseProjectFolder
        {
            get
            {
                return chooseProjectFolder.GetString(currentLanguage);
            }
        }

        public string CloseProjectQuestion
        {
            get
            {
                return closeProjectQuestion.GetString(currentLanguage);
            }
        }

        public string ProjectName
        {
            get { return projectName.GetString(currentLanguage); }
        }

        public string SiteName
        {
            get { return siteName.GetString(currentLanguage); }
        }

        public string Contractor
        {
            get { return contractor.GetString(currentLanguage); }
        }

        public string Client
        {
            get { return client.GetString(currentLanguage); }
        }

        public string NetworkType
        {
            get { return networkType.GetString(currentLanguage); }
        }

        public string FromPoint
        {
            get
            {
                return fromPoint.GetString(currentLanguage);
            }
        }

        public string ToPoint
        {
            get
            {
                return toPoint.GetString(currentLanguage);
            }
        }

        public string Length
        {
            get
            {
                return length.GetString(currentLanguage);
            }
        }

        public string Value
        {
            get
            {
                return value.GetString(currentLanguage);
            }
        }

        public string Number
        {
            get
            {
                return number.GetString(currentLanguage);
            }
        }

        public string Elevation
        {
            get
            {
                return elevation.GetString(currentLanguage);
            }
        }

        public string Toggle
        {
            get
            {
                return toggle.GetString(currentLanguage);
            }
        }

        public string Edit
        {
            get
            {
                return edit.GetString(currentLanguage);
            }
        }

        public string Processing
        {
            get
            {
                return processing.GetString(currentLanguage);
            }
        }

        public string Measurements
        {
            get
            {
                return measurements.GetString(currentLanguage);
            }
        }

        public string Benchmarks
        {
            get
            {
                return benchmarks.GetString(currentLanguage);
            }
        }

        public string Enabled
        {
            get
            {
                return enabled.GetString(currentLanguage);
            }
        }

        public string DataLoading
        {
            get
            {
                return dataLoading.GetString(currentLanguage);
            }
        }

        public string ImportExcel
        {
            get
            {
                return importExcel.GetString(currentLanguage);
            }
        }

        public string CustomImport
        {
            get
            {
                return customImport.GetString(currentLanguage);
            }
        }

        public string OverwriteDataQuestion
        {
            get
            {
                return overwriteDataQuestion.GetString(currentLanguage);
            }
        }

        public string ProcessingSuccessful
        {
            get
            {
                return successfulProcessing.GetString(currentLanguage);
            }
        }

        public string RevealQuestion
        {
            get
            {
                return revealQuestion.GetString(currentLanguage);
            }
        }

        public string ErrorOccurred
        {
            get
            {
                return errorOccurred.GetString(currentLanguage);
            }
        }

        public string EditMeasurement
        {
            get
            {
                return editMeasurement.GetString(currentLanguage);
            }
        }

        public string Save
        {
            get
            {
                return save.GetString(currentLanguage);
            }
        }

        public string Cancel
        {
            get
            {
                return cancel.GetString(currentLanguage);
            }
        }

        public string NonNumericMessage
        {
            get
            {
                return nonNumericMessage.GetString(currentLanguage);
            }
        }

        public string EditPoint
        {
            get
            {
                return editPoint.GetString(currentLanguage);
            }
        }

        public string NegativeValueMessage
        {
            get
            {
                return negativeValueMessage.GetString(currentLanguage);
            }
        }

        public string ProjectSettings
        {
            get
            {
                return projectSettings.GetString(currentLanguage);
            }
        }

        public string IOExceptionMessage
        {
            get
            {
                return ioException.GetString(currentLanguage);
            }
        }

        public string GeoAnalysisExceptionMessage
        {
            get
            {
                return incorrectGeoAnalysis.GetString(currentLanguage);
            }
        }

        public string AdjustmentResultExceptionMessage
        {
            get
            {
                return incorrectAdjustmentRes.GetString(currentLanguage);
            }
        }

        public string NetworkNotConnectedMessage
        {
            get
            {
                return networkNotConnected.GetString(currentLanguage);
            }
        }

        public string HangingPointMessagePattern
        {
            get
            {
                return hangingPoint.GetString(currentLanguage);
            }
        }

        public string UnknownErrorMessage
        {
            get
            {
                return unknownError.GetString(currentLanguage);
            }
        }

        public string DuplicateMeasurementMessagePattern
        {
            get
            {
                return duplicateMeasurement.GetString(currentLanguage);
            }
        }

        public string LoopingMeasurementsMessagePattern
        {
            get
            {
                return loopingMeasurement.GetString(currentLanguage);
            }
        }

        public string DataLoadedMessagePattern
        {
            get
            {
                return dataLoaded.GetString(currentLanguage);
            }
        }

        public string PreparingDataMessage
        {
            get
            {
                return preparingData.GetString(currentLanguage);
            }
        }

        public string PerformingGeometricAnalysis
        {
            get
            {
                return performingGeoAnalysis.GetString(currentLanguage);
            }
        }

        public string PerformingAdjustmentMessage
        {
            get
            {
                return adjusting.GetString(currentLanguage);
            }
        }

        public string CreatingReportsMessage
        {
            get
            {
                return creatingReports.GetString(currentLanguage);
            }
        }

        public string ProcessingTimeMessage
        {
            get
            {
                return processingTime.GetString(currentLanguage);
            }
        }

        public string ProjectCreatedMessagePattern
        {
            get
            {
                return createdProject.GetString(currentLanguage);
            }
        }

        public string ProjectOpenedMessagePattern
        {
            get
            {
                return openedProject.GetString(currentLanguage);
            }
        }

        public string ProjectSavedMessagePattern
        {
            get
            {
                return savedProject.GetString(currentLanguage);
            }
        }

        public string ImportScripts
        {
            get
            {
                return importScripts.GetString(currentLanguage);
            }
        }

        public string Name
        {
            get
            {
                return name.GetString(currentLanguage);
            }
        }

        public string FileExtension
        {
            get
            {
                return extension.GetString(currentLanguage);
            }
        }

        public string ScriptProperties
        {
            get
            {
                return scriptProperteis.GetString(currentLanguage);
            }
        }

        public string InputParameterType
        {
            get
            {
                return inputParameterType.GetString(currentLanguage);
            }
        }

        public string Filter
        {
            get
            {
                return filter.GetString(currentLanguage);
            }
        }

        public string ScriptFile
        {
            get
            {
                return scriptFile.GetString(currentLanguage);
            }
        }

        public string ExecutingScriptPattern
        {
            get
            {
                return executingScript.GetString(currentLanguage);
            }
        }

        public string DeleteScriptQuestion
        {
            get
            {
                return deleteScriptQuestion.GetString(currentLanguage);
            }
        }

        public string TrustScriptAuthorQuestion
        {
            get
            {
                return trustScriptSourceQustion.GetString(currentLanguage);
            }
        }

        private static LanguageViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new LanguageViewModel();
            }
            return instance;
        }

        private static string[] GetClassProperties()
        {
            Type type = typeof(LanguageViewModel);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            string[] result = new string[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                result[i] = properties[i].Name;
            }
            return result;
        }

        private void OnPropertiesChanged()
        {
            foreach (string prop in propertyNames)
            {
                if (prop != nameof(Languages))
                {
                    OnPropertyChanged(prop);
                }
            }
        }

        private static Language ParseLanguageString(string language)
        {
            if (Enum.TryParse(language, out Language lang))
            {
                return lang;
            }
            throw new ArgumentException("Incorrect enum string value was passed!");
        }
    }
}
