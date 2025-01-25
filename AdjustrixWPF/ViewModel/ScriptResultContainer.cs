using System;
using AdjustrixWPF.Model;

namespace AdjustrixWPF.ViewModel
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
