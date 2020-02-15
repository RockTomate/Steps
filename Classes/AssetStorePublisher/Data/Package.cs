using System;
using System.Collections.Generic;

namespace HardCodeLab.RockTomate.Steps.AssetStorePublisher.Data
{
    [Serializable]
    public class Package
    {
        public string PackageExportPath
        {
            get
            {
                if (this.packageExportPath == null)
                    this.packageExportPath = "Temp/uploadtool_" + this.name.Replace(' ', '_') + ".unitypackage";
                return this.packageExportPath;
            }
        }

        public string packageId; // Manually converted key to member.
        public string project_path;
        public string icon_url;
        public string root_guid;
        public string status;
        public string name;
        public bool is_complete_project;
        public string preview_url;
        public string version_name;
        public string root_path;
        //public string    assetbundles;
        public string id;
        //public string    version_id;

        public List<Project> projects = new List<Project>();
        public string[] assetPaths;

        private string packageExportPath;

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("[{0}] [{1}] {2} | {3}", packageId, status, name, project_path);
        }
    }
}