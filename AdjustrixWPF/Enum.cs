using System;
using System.Collections.ObjectModel;

namespace AdjustrixWPF
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


    public static class EnumHelper
    {
        public static ObservableCollection<string> GetEnumStrings<T>()
            where T : Enum
        {
            var enumValues = Enum.GetNames(typeof(T));
            return new ObservableCollection<string>(enumValues);
        }
    }
}
