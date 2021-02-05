#if NET_4_6 || NET_STANDARD_2_0

using LibGit2Sharp;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    public abstract class BaseGitStep : SimpleStep
    {
        [InputField(required: true, tooltip: "Directory path where target repository is located.")]
        public string RepositoryPath;

        protected Repository GetRepository()
        {
            return new Repository(RepositoryPath);
        }
    }
}

#endif