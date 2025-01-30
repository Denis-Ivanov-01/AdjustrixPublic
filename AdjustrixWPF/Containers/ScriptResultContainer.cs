using System;
using AdjustrixWPF.PythonScripting;

namespace AdjustrixWPF.Containers
{
    public class ScriptResultContainer
    {
        public event Action<ScriptResult> ResultChanged;

        public void ChangeScriptResult(ScriptResult result)
        {
            ResultChanged?.Invoke(result);
        }
    }
}
