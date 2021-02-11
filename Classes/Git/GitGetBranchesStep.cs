#if NET_4_6 || NET_STANDARD_2_0

using System.Collections.Generic;
using HardCodeLab.RockTomate.Core.Attributes;
using LibGit2Sharp;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Git - Get Branches", "Retrieves all branches of target repository", StepCategories.GitCategory)]
    public class GitGetAllBranchesStep : BaseGitStep
    {
        [OutputField(tooltip: "Name of a currently checked out branch.")]
        public string CurrentBranch;

        [OutputField(tooltip: "Array of all branch names.")]
        public List<string> AllBranches = new List<string>();

        [OutputField(tooltip: "Array of remote branch names.")]
        public List<string> RemoteBranches = new List<string>();

        protected override bool OnStepStart()
        {
            using (var repo = GetRepository())
            {
                foreach (var branch in repo.Branches)
                {
                    var branchName = branch.FriendlyName;

                    AllBranches.Add(branchName);

                    if (branch.IsRemote)
                        RemoteBranches.Add(branchName);

                    if (branch.IsCurrentRepositoryHead)
                        CurrentBranch = branchName;
                }
            }

            return true;
        }
    }
}

#endif