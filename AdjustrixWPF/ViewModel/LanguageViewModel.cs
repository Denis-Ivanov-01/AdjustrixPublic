using System.Collections.ObjectModel;

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
        private StringEntry file = new("Файл", "File");
        private StringEntry data = new("Данни", "Data");
        private StringEntry appearance = new("Изглед", "Appearance");
        private Language currentLanguage;

        public LanguageViewModel()
        {
            Languages = new();
            Languages.Add("Bulgarian");
            Languages.Add("English");
            //todo: later load it from AppData/Local
            currentLanguage = Language.English;
        }

        public string CurrentLanguage
        {
            get
            {
                return currentLanguage.ToString();
            }
            set
            {
                currentLanguage = Language.English.ToString() == value ? Language.English : Language.Bulgarian;
                OnPropertyChanged(nameof(File));
                OnPropertyChanged(nameof(Data));
                OnPropertyChanged(nameof(Appearance));
                OnPropertyChanged("");
                OnPropertyChanged();
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

        public ObservableCollection<string> Languages { get; set; }
    }
}
