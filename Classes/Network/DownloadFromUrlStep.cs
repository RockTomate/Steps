using System;
using System.Net;
using System.Collections;
using System.ComponentModel;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Download from URL", "Downloads a file from a given URL.", StepCategories.NetworkCategory)]
    public class DownloadFromUrlStep : Step
    {
        private WebClient _webClient;
        private int _currentProgress;
        private bool _operationFinished;

        [InputField("URL", "URL to download files from", required: true)]
        public string Url;

        [InputField("Destination File Path", "Destination directory where downloaded file will be placed", required: true)]
        public string DestinationFilePath;

        /// <inheritdoc />
        protected override void OnInterrupt()
        {
            _webClient.CancelAsync();
            _webClient.Dispose();
        }

        /// <inheritdoc />
        protected override void OnReset()
        {
            _currentProgress = 0;
            _operationFinished = false;
        }

        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            _webClient = new WebClient();
            _webClient.DownloadProgressChanged += OnDownloadPressChanged;
            _webClient.DownloadFileCompleted += OnDownloadFinish;

            _webClient.DownloadFileAsync(new Uri(Url), DestinationFilePath);

            while (!_operationFinished)
                yield return null;

            _webClient.Dispose();
        }

        private void OnDownloadFinish(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error.GetType().Name == "WebException")
            {
                IsSuccess = false;
                var webException = (WebException)e.Error;
                var response = (HttpWebResponse)webException.Response;

                RockLog.WriteLine(this, LogTier.Error,
                    string.Format("Error occurred (response code \"{0}\") when trying to download a file from: \"{1}\"", response.StatusCode, Url));
            }

            _operationFinished = true;
        }

        private void OnDownloadPressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _currentProgress = e.ProgressPercentage;
        }

        /// <inheritdoc />
        public override string InProgressText
        {
            get { return string.Format("{0}%", _currentProgress); }
        }
    }
}