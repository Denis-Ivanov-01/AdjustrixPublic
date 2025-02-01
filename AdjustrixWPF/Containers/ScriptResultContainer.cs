using System;
using AdjustrixWPF.PythonScripting;

namespace AdjustrixWPF.Containers
{
    /// <summary>
    /// Supposed to be used only by the PythonProcessManager - publisher
    /// ImportScriptViewModel - listener
    /// </summary>
    public class ScriptResultContainer
    {
        public event Action<ScriptResult> ResultChanged;

        public void ChangeScriptResult(ScriptResult result)
        {
            ResultChanged?.Invoke(result);
        }
    }
}
