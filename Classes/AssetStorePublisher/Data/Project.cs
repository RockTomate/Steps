using System;

namespace HardCodeLab.RockTomate.Steps.AssetStorePublisher.Data
{
    [Serializable]
    public class Project
    {
        public string projectPath = string.Empty;
        public string rootGUID = string.Empty;
        public string rootPath = string.Empty;
        public string packageExportPath = string.Empty;
        public string[] assetPaths;
    }
}