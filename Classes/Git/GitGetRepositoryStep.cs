#if NET_4_6 || NET_STANDARD_2_0

using LibGit2Sharp;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Git - Get Repository", "Loads repository", StepCategories.GitCategory)]
    public class GitGetRepositoryStep : SimpleStep
    {
        [InputField(tooltip: "Directory path where target repository is located.", required: true)]
        public string RepositoryPath;

        [OutputField("Repository", "Repository resource")]
        public IRepository LoadedRepository;

        protected override bool OnValidate()
        {
            if (!Repository.IsValid(RepositoryPath))
            {
                RockLog.WriteLine(this, LogTier.Error, $"Repository at \'{RepositoryPath}\' does not exist.");
                return false;
            }

            return true;
        }

        protected override bool OnStepStart()
        {
            LoadedRepository = new Repository(RepositoryPath);
            return true;
        }
    }
}

#endif