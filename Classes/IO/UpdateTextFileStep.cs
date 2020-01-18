using System;
using System.IO;
using System.Text;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Update Text File", "Updates contents of a text file (or creates a new one if it doesn't exist).", StepCategories.FileSystemCategory)]
    public class UpdateTextFileStep : SimpleStep
    {
        [InputField(tooltip: "Path to a file that will be affected", required: true)]
        public string FilePath;

        [InputField(tooltip: "Contents of the new file")]
        public string[] Contents;

        [InputField(tooltip: "Whether to add a newline at the end an individual content or not")]
        public WriteType WriteMode = WriteType.WriteLine;

        [InputField(tooltip: "Specifies what to do if file already exists")]
        public DuplicateBehaviour DuplicateFileBehaviour = DuplicateBehaviour.Fail;

        public enum DuplicateBehaviour
        {
            Fail,
            Overwrite,
            Append,
        }

        public enum WriteType
        {
            Write,
            WriteLine,
        }

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            if (DuplicateFileBehaviour == DuplicateBehaviour.Fail && File.Exists(FilePath))
                return false;

            return true;
        }

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            try
            {
                if (DuplicateFileBehaviour == DuplicateBehaviour.Overwrite && File.Exists(FilePath))
                    File.Delete(FilePath);

                using (var streamWriter = new StreamWriter(FilePath, DuplicateFileBehaviour == DuplicateBehaviour.Append, Encoding.UTF8))
                {
                    foreach (var content in Contents)
                    {
                        switch (WriteMode)
                        {
                            case WriteType.Write:

                                streamWriter.Write(content);

                                break;
                            case WriteType.WriteLine:

                                streamWriter.WriteLine(content);

                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                RockLog.WriteLine(LogTier.Error, ex.Message);
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        protected override string Description
        {
            get
            {
                return string.Format("Create a text file with contents at \"{0}\"", FilePath);
            }
        }
    }
}