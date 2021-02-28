#if NET_4_6 || NET_STANDARD_2_0

using LibGit2Sharp;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Git - Checkout Branch", "Changes local branch.", StepCategories.GitCategory)]
    public class GitCheckoutBranchStep : BaseGitStep
    {
        [InputField(tooltip: "Name of the local branch that repository will switch to", required: true)]
        public string BranchName;

        protected override bool OnStepStart()
        {
            var branch = Repository.Branches[BranchName];

            if (branch == null)
            {
                RockLog.WriteLine(this, LogTier.Error, $"Branch of name \"{BranchName}\" does not exist.");
                return false;
            }

            Commands.Checkout(Repository, branch);

            return true;
        }
    }
}

#endif