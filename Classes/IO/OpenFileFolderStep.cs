using System.IO;
using UnityEngine;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Open File/Folder", "Opens a file/folder with a default program.", StepCategories.FileSystemCategory)]
    public class OpenFileFolderStep : SimpleStep
    {
        [InputField(tooltip: "Path to a file or directory which will be opened.", required: true)]
        public string Path;

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            if (!File.Exists(Path) && !Directory.Exists(Path))
                return false;

            return true;
        }

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            Application.OpenURL(string.Format("file://{0}", Path));
            return true;
        }

        /// <inheritdoc />
        protected override string Description
        {
            get { return string.Format("Open a \"{0}\"", Path); }
        }
    }
}