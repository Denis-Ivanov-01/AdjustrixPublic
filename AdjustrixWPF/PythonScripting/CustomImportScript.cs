using AdjustrixWPF.Enums;

namespace AdjustrixWPF.PythonScripting
{
    public class CustomImportScript
    {
        public string Name { get; set; }

        public ScriptParameterType ScriptParameterType { get; set; }

        public string FileExtension { get; set; }

        public string FileFilter { get; set; }

        public string ScriptPath { get; set; }
    }
}
