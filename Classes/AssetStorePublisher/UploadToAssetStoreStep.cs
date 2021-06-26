/*
 *  Made possible by Michael "Mikilo" Nguyen
*/

using System.Net;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEditor;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Helpers;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Steps.AssetStorePublisher;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Upload to Asset Store", "Logs into publisher account and uploads an asset to the Asset Store Publisher Dashboard.",
        StepCategories.AssetStorePublisherCategory)]
    public class UploadToAssetStoreStep : Step
    {
        private const string CredentialsCategory = "Credentials";
        private const string PackageCategory = "Package";

        private AssetStoreAPI _assetStoreApi;

        [InputField(tooltip: "Package Id that will be uploaded to.", required: true)]
        public string PackageId;

        [InputField(tooltip: "Username/Email of Unity Asset Store account.", required: true, sensitive: true, category: CredentialsCategory)]
        public string Username;

        [InputField(tooltip: "Unity Asset Store password", required: true, sensitive: true, category: CredentialsCategory)]
        public string Password;

        [InputField(tooltip: "Directory path to your asset.\n" +
                             "Must be relative to project directory.\n" +
                             "Must NOT begin with \"Assets/\". Must start with \"/\" (forward slash) instead.", 
            required: true, category: PackageCategory)]
        public string AssetDirectoryPath;

        [InputField(tooltip: "In addition to the assets paths listed, all dependent assets will be included as well.", category: PackageCategory)]
        public bool IncludeDependencies = true;

        [InputField(tooltip: "The exported package will include all library assets, ie. the project settings located in the Library folder of the project.", category: PackageCategory)]
        public bool IncludeLibraryAssets = false;

        /// <inheritdoc />
        protected override void OnPostExecute()
        {
            _assetStoreApi.Logout();
            _assetStoreApi = null;
        }

        /// <inheritdoc />
        protected override void OnInterrupt()
        {
            _assetStoreApi.Logout();
            _assetStoreApi = null;
        }

        /// <summary>
        /// Generates a temporary package that will be uploaded.
        /// </summary>
        /// <returns>Returns a path to a generated package.</returns>
        private string GeneratePackage()
        {
            var exportFilePath = PathHelpers.Combine(MakeTempDir(), string.Format("temp_package_{0}.unitypackage", PackageId));
            var exportOptions = ExportPackageOptions.Default | ExportPackageOptions.Recurse;

            if (IncludeDependencies)
                exportOptions |= ExportPackageOptions.IncludeDependencies;

            if (IncludeLibraryAssets)
                exportOptions |= ExportPackageOptions.IncludeLibraryAssets;

            var sourceRootDirectory = PathHelpers.Combine("Assets/", AssetDirectoryPath);

            AssetDatabase.ExportPackage(sourceRootDirectory, exportFilePath, exportOptions);

            return exportFilePath;
        }

        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            AssetDirectoryPath = PathHelpers.FixSlashes(AssetDirectoryPath);


            _assetStoreApi = new AssetStoreAPI();

            yield return null;
            _assetStoreApi.Login(Username, Password);

            while (_assetStoreApi.IsLogging())
                yield return null;

            IsSuccess = _assetStoreApi.IsConnected;

            if (!IsSuccess)
            {
                RockLog.WriteLine(this, LogTier.Error, _assetStoreApi.LastError);
                yield break;
            }

            _assetStoreApi.FetchPackages();

            while (_assetStoreApi.IsFetchingPackages())
                yield return null;

            IsSuccess = _assetStoreApi.HasPackages;

            if (!IsSuccess)
            {
                RockLog.WriteLine(this, LogTier.Error, _assetStoreApi.LastError);
                yield break;
            }

            // fetch required package
            var package = _assetStoreApi.EachPackage.FirstOrDefault(x => x.packageId == PackageId);

            if (package == null)
            {
                RockLog.WriteLine(this, LogTier.Error, string.Format("Package of id \"{0}\" not found.", PackageId));
                IsSuccess = false;
                yield break;
            }

            yield return null;

            var packageFilePath = GeneratePackage();

            yield return null;

            HttpWebResponse uploadWebResponse = null;

            var rootPath = AssetDirectoryPath;

            if (!rootPath.StartsWith("/"))
                rootPath = '/' + AssetDirectoryPath;

            // upload package to the asset store dashboard
            _assetStoreApi.Upload(
                package.id,
                packageFilePath,
                rootPath,
                Application.dataPath,
                Application.unityVersion,
                (response, s) =>
                {
                    uploadWebResponse = response;
                }
            );

            while (uploadWebResponse == null)
                yield return null;

            IsSuccess = uploadWebResponse.StatusCode == HttpStatusCode.OK;
        }
    }
}