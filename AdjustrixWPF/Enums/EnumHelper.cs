using System.Collections.ObjectModel;

namespace AdjustrixWPF.Enums
{
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
