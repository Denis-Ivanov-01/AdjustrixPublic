using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AdjustrixWPF.View
{
    public class ThemeManager : DependencyObject
    {
public static readonly DependencyProperty CurrentThemeDictionaryProperty =
         DependencyProperty.RegisterAttached("CurrentThemeDictionary", typeof(Uri),
         typeof(ThemeManager),
         new UIPropertyMetadata(null, CurrentThemeDictionaryChanged));

        public static Uri GetCurrentThemeDictionary(DependencyObject obj)
        {
            return (Uri)obj.GetValue(CurrentThemeDictionaryProperty);
        }

        public static void SetCurrentThemeDictionary(DependencyObject obj, Uri value)
        {
            obj.SetValue(CurrentThemeDictionaryProperty, value);
        }

        private static void CurrentThemeDictionaryChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is FrameworkElement) // works only on FrameworkElement objects
            {
                ApplyTheme(obj as FrameworkElement, GetCurrentThemeDictionary(obj));
            }
        }

        private static void ApplyTheme(FrameworkElement targetElement, Uri dictionaryUri)
        {
            if (targetElement == null)
            {
                return;
            }

            try
            {
                ResourceDictionary themeDictionary = null;
                if (dictionaryUri != null)
                {
                    themeDictionary = new();
                    themeDictionary.Source = dictionaryUri;

                    // add the new dictionary to the collection of merged dictionaries of the target object
                    Application.Current.Resources.MergedDictionaries.Insert(0, themeDictionary);
                }

                // find if the target element already has a theme applied



                //System.Collections.Generic.List<object> existingDictionaries =
                //    (from dictionary in targetElement.Resources.MergedDictionaries.W(typeof(ThemeResourceDictionary))
                //     select dictionary).ToList();
                //List<ResourceDictionary> existingDictionaries = targetElement.Resources.MergedDictionaries
                //    .Where(md => md.Source.AbsolutePath.Contains("Theme")).ToList();

                List<ResourceDictionary> existingDictionaries = Application.Current.Resources.MergedDictionaries
                    .Where(md => md.Source != null && md.Source.OriginalString.Contains("Theme") &&
                    !md.Source.OriginalString.Contains("MaterialDesign")).ToList();

                // remove the existing dictionaries
                foreach (ResourceDictionary thDictionary in existingDictionaries)
                {
                    if (themeDictionary.Source == thDictionary.Source)
                    {
                        continue;  // don't remove the newly added dictionary
                    }

                    Application.Current.Resources.MergedDictionaries.Remove(thDictionary);
                }

                //List<ResourceDictionary> materialDesignThemes = Application.Current.Resources.MergedDictionaries
                //    .Where(md => md.GetType() == typeof(BundledTheme)).ToList();
                //foreach (ResourceDictionary theme in materialDesignThemes)
                //{
                //    Application.Current.Resources.MergedDictionaries.Remove(theme);
                //}
                //Theme currentTheme = (Theme)Enum.Parse(typeof(Theme), ThemeViewModel.SelectedTheme);
                //if (currentTheme == Theme.Dark)
                //{
                //    Application.Current.Resources.MergedDictionaries.Insert(0, darkTheme);
                //}
                //else
                //{
                //    Application.Current.Resources.MergedDictionaries.Insert(0, lightTheme);
                //}
            }
            finally { }
        }
    }
}
