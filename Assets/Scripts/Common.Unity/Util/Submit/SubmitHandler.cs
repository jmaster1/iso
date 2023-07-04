using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Common.IO.FileSystem;
using Common.IO.Serialize;
using Common.Unity.Bind;
using Common.Unity.Boot;
using Common.Unity.UI.Debug;
using Common.Util;
using UnityEngine;

namespace Common.Unity.Util.Submit
{
    /// <summary>
    /// listens to application error and submits report
    /// and/or displays error dialog (depending on debug flag)
    /// in debug mode can export/import state from server
    /// </summary>
    public class SubmitHandler : BindableMono<object>
    {
        private const string HeaderAppKey = "app-key";
        private const string HeaderAppId = "app-id";
        
        [Tooltip("error server url base")]
        public string urlBase;
        
        [Tooltip("application secure key")]
        public string appKey;

        /// <summary>
        /// http client
        /// </summary>
        private HttpClient client;

        private HttpClient Client
        {
            get
            {
                if (client != null) return client;
                client = new HttpClient();
                client.DefaultRequestHeaders.Add(HeaderAppId, Application.identifier);
                client.DefaultRequestHeaders.Add(HeaderAppKey, appKey);
                return client;
            }
        }
        
        /// <summary>
        /// filesystem which content to submit in a zip
        /// </summary>
        public AbstractFileSystem FileSystem;

        /// <summary>
        /// optional filter for files to submit
        /// </summary>
        public Func<string, bool> FileFilter;
        
        /// <summary>
        /// shows whether error encountered
        /// </summary>
        public bool ErrorEncountered { get; private set; }
        
        /// <summary>
        /// shows whether error report sent in current session 
        /// </summary>
        public bool ErrorReportSent { get; private set; }
        
        /// <summary>
        /// currently visible error dialog (to prevent multiple instances)
        /// </summary>
        DialogMessage errorDialog;

        private void Awake()
        {
            FileSystem = UnityHelper.PrivateFileSystem;
            Application.logMessageReceived += OnApplicationLogMessageReceived;
            if (Unicom.IsDebug)
            {
                Unicom.RunNextTime(() =>
                {
                    Unicom.Debug.AddTab<SubmitHandler, SubmitDebugView>(this, "submit");
                });
            }
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= OnApplicationLogMessageReceived;
        }

        private void OnApplicationLogMessageReceived(string condition, string stacktrace, LogType type)
        {
            if(!Application.isPlaying) return;
            switch (type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    OnError(type, condition, stacktrace);
                    break;
            }
        }

        private void OnError(LogType type, string condition, string stacktrace)
        {
            ErrorEncountered = true;
            //
            // rip useless data from stacktrace
            stacktrace = stacktrace?.Replace(" (at <00000000000000000000000000000000>:0)", StringHelper.EmptyString);
            Log.Error($"Unhandled {type}, condition={condition}, stacktrace={stacktrace}");
            //
            // show dialog on debug, otherwise submit
            if (Unicom.IsDebug)
            {
                ShowErrorDialog(type, condition, stacktrace);
            } else if (!ErrorReportSent)
            {
                ErrorReportSent = true;
                SubmitReport(type, condition, stacktrace);
            }
        }

        private async void SubmitReport(LogType type, string condition, string stacktrace, bool showResult = false)
        {
            var report = CreateErrorReport();
            report.errorType = type.ToString();
            report.errorCondition = condition;
            report.errorStacktrace = stacktrace;
            NewtonsoftJsonObjectSerializer.Instance.Write(report, FileSystem);
            var result = await ExportState();
            if (!showResult) return;
            var dmResult = new DialogMessage
            {
                Title = "Submit report result",
                Message = result,
            };
            dmResult.AddAction("Ok", LangHelper.VoidAction);
            dmResult.Show();
        }

        public async Task<string> ExportState()
        {
            using var ms = new MemoryStream();
            using var archive = new ZipArchive(ms, ZipArchiveMode.Create);
            var zip = new ZipFileSystem(archive);
            FileSystem.CopyTo(zip, FileFilter);
            archive.Dispose();
            var url = urlBase + "post";
            var bytes = ms.GetBuffer();
            var content = new ByteArrayContent(bytes);
            content.Headers.Add("Content-Type", "application/zip");
            var response = await Client.PostAsync(url, content);
            var result = response.Content.ReadAsStringAsync().Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode + ": " + result);
            }
            return result;
        }
        
        public async Task ImportState(string submitId)
        {
            var url = urlBase + submitId;
            var response = await Client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode + ": " + response.Content.ReadAsStringAsync().Result);
            }
            var data = await response.Content.ReadAsByteArrayAsync();
            using var ms = new MemoryStream(data);
            using var archive = new ZipArchive(ms, ZipArchiveMode.Read);
            var zip = new ZipFileSystem(archive);
            FileSystem.DeleteAll(FileFilter);
            zip.CopyTo(FileSystem);
            Unicom.Instance.Reload();
        }

        private void ShowErrorDialog(LogType type, string condition, string stacktrace)
        {
            if(errorDialog != null) return;
            errorDialog = new DialogMessage
            {
                Title = type.ToString(),
                Message = condition + StringHelper.EOL + stacktrace,
            };
            errorDialog.AddAction("Submit report", () =>
            {
                SubmitReport(type, condition, stacktrace, true);
            });
            errorDialog.AddAction("Cancel", LangHelper.VoidAction);
            errorDialog.OnClose += () => { errorDialog = null; };
            errorDialog.Show();
        }

        public ErrorReport CreateErrorReport()
        {
            var e = new ErrorReport
            {
                debug = Unicom.IsDebug,
                systemTime = DateTime.Now,
                gameTime = Unicom.GameTime.ValueSeconds,
                appId = Application.identifier,
                appVersion = Application.version,
                appGenuine = Application.genuine,
                appPlatform = Application.platform.ToString(),
                appInstaller = Application.installerName,
                appInstallMode = Application.installMode.ToString(),
                systemLang = Application.systemLanguage.ToString(),
                deviceModel = SystemInfo.deviceModel,
                operatingSystem = SystemInfo.operatingSystem,
                deviceUid = SystemInfo.deviceUniqueIdentifier,
                systemMemorySizeMb = SystemInfo.systemMemorySize
            };
            return e;
        }
    }
}
