﻿using System.IO;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Helpers;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Delete Directory", "Deletes a directory", StepCategories.FileSystemCategory)]
    public class DeleteDirectoryStep : SimpleStep
    {
        [InputField(tooltip: "Path of a directory which will be deleted.", required: true)]
        public string DirectoryPath;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            if (Directory.Exists(DirectoryPath))
            {
                PathHelpers.DeleteDirectory(DirectoryPath);
            }

            return true;
        }

        /// <inheritdoc />
        protected override string Description
        {
            get { return string.Format("Delete the directory at \"{0}\"", DirectoryPath); }
        }
    }
}