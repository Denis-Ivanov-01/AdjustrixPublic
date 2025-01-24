using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using Adjustment.Project;

namespace AdjustrixWPF.ViewModel
{
    class ProjectTypeConverter : IValueConverter
    {
        private const string levelingBg = "Нивелачна";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<ProjectType> types)
            {
                ObservableCollection<string> result = new();
                foreach (ProjectType type in types)
                {
                    result.Add(ConvertType(type));
                }
                return result;
            }
            else if (value is IEnumerable<string> typesStr)
            {
                ObservableCollection<string> result = new();
                foreach (string type in typesStr)
                {
                    result.Add(ConvertType(type));
                }
                return result;
            }
            else if (value is string type)
            {
                return ConvertType(type);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<string> types)
            {
                ObservableCollection<string> result = new();
                foreach (string type in types)
                {
                    result.Add(ConvertTypeBack(type));
                }
                return result;
            }
            else if (value is string type)
            {
                return ConvertTypeBack(type);
            }
            return value;
        }

        private string ConvertType(ProjectType type)
        {
            Language language = LanguageViewModel.SelectedLanguage;
            if (language == Language.English)
            {
                return type.ToString();
            }
            if (type == ProjectType.Leveling)
            {
                return levelingBg;
            }
            throw new NotImplementedException($"A non-suported project type was passed: {type.ToString()}");
        }

        private string ConvertType(string type)
        {
            if (LanguageViewModel.SelectedLanguage == Language.English) { return type; }
            if (type == ProjectType.Leveling.ToString()) { return levelingBg; }
            throw new ArgumentException("$A non-suported project type was passed: {type}");
        }

        private string ConvertTypeBack(string type)
        {
            if (type == levelingBg || type == ProjectType.Leveling.ToString())
            {
                return ProjectType.Leveling.ToString();
            }
            throw new ArgumentException($"A project type string that cannot be converted to ProjectType was passed {type}");
        }
    }
}
