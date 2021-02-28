#if NET_4_6 || NET_STANDARD_2_0

using System.Linq;
using System.Collections.Generic;
using LibGit2Sharp;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Git - Get Branch History", "Retrieves branch history.", StepCategories.GitCategory)]
    public class GitGetBranchHistoryStep : BaseGitStep
    {
        [InputField(tooltip: "Name of the branch which history will be returned.", required: true)]
        public string BranchName;

        [InputField(tooltip: "Specifies number of recent commits to return.\n" +
                             "If 0 or less than 0, the entire commit history will be returned.", required: true)]
        public int CommitCount = 10;

        [OutputField(tooltip: "A list of entire commit history")]
        public IEnumerable<Commit> CommitHistory;

        protected override bool OnStepStart()
        {
            CommitHistory = CommitCount > 0
                ? Repository.Commits.Take(CommitCount)
                : Repository.Commits;

            return true;
        }
    }
}

#endif