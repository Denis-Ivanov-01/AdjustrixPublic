using System.Collections.ObjectModel;

namespace AdjustrixWPF.Enums
{


    public enum Language
    {
        English,
        Bulgarian
    }

    public enum Theme
    {
        Dark,
        Light
    }

    public enum ScriptParameterType
    {
        FilePath,
        Directory
    }

    public enum YesNoPromptResult
    {
        Yes,
        No
    }

    public static class EnumHelper
    {
        public static ObservableCollection<string> GetEnumStrings<T>()
            where T : System.Enum
        {
            var enumValues = System.Enum.GetNames(typeof(T));
            return new ObservableCollection<string>(enumValues);
        }
    }
}
