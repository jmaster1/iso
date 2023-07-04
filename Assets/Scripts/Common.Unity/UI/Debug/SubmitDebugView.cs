using System;
using System.Threading.Tasks;
using Common.Unity.Bind;
using Common.Unity.Util.Submit;
using Common.Util;
using TMPro;
using UnityEngine.UI;

namespace Common.Unity.UI.Debug
{
    /// <summary>
    /// ui for export/import state
    /// </summary>
    public class SubmitDebugView : BindableMono<SubmitHandler>
    {
        public TMP_InputField submitInput;
        
        public TMP_InputField log;
        
        public Button clearButton;
        
        public Button exportButton;
        
        public Button importButton;
        
        public override void OnBind()
        {
            BindClick(clearButton, () => log.text = null);
            BindClick(exportButton, () => DoAsync(ExportState));
            BindClick(importButton, () => DoAsync(ImportState));
        }

        private void SetInteractible(bool b)
        {
            submitInput.interactable = exportButton.interactable = importButton.interactable = b;
        }
        
        private void LogAppend(string line)
        {
            log.text += line + StringHelper.EOL;
        }

        private async void DoAsync(Func<Task> task)
        {
            SetInteractible(false);
            try
            {
                await task.Invoke();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex);
                LogAppend(ex.ToString());
            }
            finally
            {
                SetInteractible(true);
            }
        }

        private async Task ExportState()
        {
            LogAppend("Exporting...");
            var result = await Model.ExportState();
            submitInput.text = result;
            LogAppend($"Success (submit id = {result})");
        }

        private async Task ImportState()
        {
            var submitId = submitInput.text;
            LogAppend($"Importing (submit id = {submitId})...");
            await Model.ImportState(submitId);
            LogAppend("Success");
        }
    }
}
