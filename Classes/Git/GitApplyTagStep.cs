#if NET_4_6 || NET_STANDARD_2_0

using LibGit2Sharp;
using HardCodeLab.RockTomate.Core.VC.Git;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Git - Apply Tag", "Applies a tag on latest commit on current branch.", StepCategories.GitCategory)]
    public class GitApplyTagStep : BaseGitStep
    {
        [InputField(required: true)]
        public string TagName;

        [InputField(tooltip: "Commit onto which tag will be applied.\n" +
                             "If empty or 0, latest commit will be retrieved.\n" +
                             "If -N, N to last commit will be retrieved (where N is a number)\n" +
                             "Alternatively, a commit hash can be supplied directly (searches globally)")]
        public string CommitSearch;

        protected override bool OnStepStart()
        {
            var commit = GitUtils.GetCommit(Repository, CommitSearch);
            if (commit == null)
            {
                RockLog.WriteLine(this, LogTier.Error, "Commit not found!");
                return false;
            }

            Repository.ApplyTag(TagName, commit.Sha);

            return true;
        }
    }
}

#endif