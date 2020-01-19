using System;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;
using UnityEngine;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Download from URL", "Downloads a file from a given URL.", StepCategories.NetworkCategory)]
    public class DownloadFromUrlStep : Step
    {
        private UnityWebRequest _unityWebRequest;

        [InputField("URL", "URL to download files from", required: true)]
        public string Url;

        [InputField("Destination File Path", "Destination directory where downloaded file will be placed", required: true)]
        public string DestinationFilePath;

        [InputField(category: "Advanced", tooltip: "Indicates the number of redirects which this request will follow before halting with a “Redirect Limit Exceeded” system error.\n" +
                             "If you do not wish to limit the number of redirects, you may set this property to any negative number. (NOT RECOMMENDED)")]
        public int RedirectLimit = 32;

        [InputField(category: "Advanced", tooltip: "Indicates whether the UnityWebRequest system should employ the HTTP/1.1 chunked-transfer encoding method.")]
        public bool UseChunkedTransfer = false;

        /// <inheritdoc />
        protected override void OnInterrupt()
        {
            _unityWebRequest.Abort();
            _unityWebRequest.Dispose();
        }

        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DestinationFilePath) ?? "");

            _unityWebRequest = UnityWebRequest.Get(new Uri(Url).AbsoluteUri);
            _unityWebRequest.redirectLimit = RedirectLimit;
            Debug.Log(_unityWebRequest.chunkedTransfer);
            _unityWebRequest.chunkedTransfer = UseChunkedTransfer;

            var operation = _unityWebRequest.SendWebRequest();

            while (!operation.isDone)
                yield return null;

            // handle failure scenario
            if (_unityWebRequest.isNetworkError || _unityWebRequest.isHttpError)
            {
                RockLog.WriteLine(this, LogTier.Error, _unityWebRequest.error);
                IsSuccess = false;
                yield break;
            }
            // if the download was successful, save it
            else
            {
                RockLog.WriteLine(this, LogTier.Debug, string.Format("Finished downloading file. Saving at \"{0}\"...", DestinationFilePath));

                File.WriteAllBytes(DestinationFilePath, _unityWebRequest.downloadHandler.data);

                RockLog.WriteLine(this, LogTier.Debug, string.Format("Finished saving at \"{0}\"!", DestinationFilePath));

                IsSuccess = true;
            }
            _unityWebRequest.Dispose();
        }

        /// <inheritdoc />
        public override string InProgressText
        {
            get { return "Downloading"; }
        }
    }
}