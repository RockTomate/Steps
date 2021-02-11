#if NET_4_6 || NET_STANDARD_2_0

using LibGit2Sharp;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Git - Init Repository", "Initialize a repository.", StepCategories.GitCategory)]
    public class GitInitStep : SimpleStep
    {
        [InputField(tooltip: "Directory path where Git repository will be initialized.", required: true)]
        public string NewRepositoryPath;

        protected override bool OnValidate()
        {
            if (Repository.IsValid(NewRepositoryPath))
            {
                RockLog.WriteLine(this, LogTier.Error, $"Repository at \'{NewRepositoryPath}\' already exists.");
                return false;
            }

            return true;
        }

        protected override bool OnStepStart()
        {
            Repository.Init(NewRepositoryPath);
            return true;
        }
    }
}

#endif