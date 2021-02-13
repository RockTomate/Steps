#if NET_4_6 || NET_STANDARD_2_0

using System.Linq;
using System.Collections.Generic;
using LibGit2Sharp;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Git - Compare Commits", "Get file differences between 2 commits of the repository.", StepCategories.GitCategory)]
    public class GitCompareCommitsStep : BaseGitStep
    {
        private const string AllCategory = "All";
        private const string AddedCategory = "Added only";
        private const string DeletedCategory = "Deleted only";
        private const string RenamedCategory = "Renamed only";
        private const string ModifiedCategory = "Modified only";
        private const string ConflictedCategory = "Conflicted only";

        [InputField(tooltip: "Commit 1.\n" +
                             "If empty or 0, latest commit will be retrieved.\n" +
                             "If -N, N to last commit will be retrieved (where N is a number)\n" +
                             "Alternatively, a commit hash can be supplied directly (searches globally)")]
        public string CommitSearch1;

        [InputField(tooltip: "Commit 2.\n" +
                             "If empty or 0, latest commit will be retrieved.\n" +
                             "If -N, N to last commit will be retrieved (where N is a number)\n" +
                             "Alternatively, a commit hash can be supplied directly (searches globally)")]
        public string CommitSearch2 = "-1";

        [OutputField(tooltip: "Array of file paths that were affected (added, deleted, renamed etc.).", category: GitCompareCommitsStep.AllCategory)]
        public List<string> FilePaths = new List<string>();

        [OutputField(tooltip: "Array of file paths that were added.", category: GitCompareCommitsStep.AddedCategory)]
        public List<string> AddedFilePaths = new List<string>();

        [OutputField(tooltip: "Array of file paths that were deleted.", category: GitCompareCommitsStep.DeletedCategory)]
        public List<string> DeletedFilePaths = new List<string>();

        [OutputField(tooltip: "Array of file paths that were renamed.", category: GitCompareCommitsStep.RenamedCategory)]
        public List<string> RenamedFilePaths = new List<string>();

        [OutputField(tooltip: "Array of file paths which contents were modified.", category: GitCompareCommitsStep.ModifiedCategory)]
        public List<string> ModifiedFilePaths = new List<string>();

        [OutputField(tooltip: "Array of file paths which contents are in merge conflict.", category: GitCompareCommitsStep.ConflictedCategory)]
        public List<string> ConflictedFilePaths = new List<string>();

        protected override bool OnStepStart()
        {
            using (var repo = GetRepository())
            {
                var commit1 = GitStepsUtils.GetCommit(repo, CommitSearch1);
                if (commit1 == null)
                {
                    RockLog.WriteLine(this, LogTier.Error, "Commit 1 not found!");
                    return false;
                }

                RockLog.WriteLine(this, LogTier.Info, $"Commit 1 found ({commit1.Sha})");

                var commit2 = GitStepsUtils.GetCommit(repo, CommitSearch2);
                if (commit2 == null)
                {
                    RockLog.WriteLine(this, LogTier.Error, "Commit 2 not found!");
                    return false;
                }

                RockLog.WriteLine(this, LogTier.Info, $"Commit 2 found ({commit2.Sha})");

                if (commit1 == commit2)
                {
                    RockLog.WriteLine(this, LogTier.Warning, "Both commits are the same. Aborting comparison...");
                    return true;
                }

                var commitTree1 = commit1.Parents.First().Tree;
                var commitTree2 = commit2.Parents.First().Tree;

                foreach (var entryChange in repo.Diff.Compare<TreeChanges>(commitTree1, commitTree2))
                {
                    var filePath = entryChange.Path;

                    FilePaths.Add(filePath);

                    switch (entryChange.Status)
                    {
                        case ChangeKind.Added:
                            AddedFilePaths.Add(filePath);
                            break;
                        case ChangeKind.Deleted:
                            DeletedFilePaths.Add(filePath);
                            break;
                        case ChangeKind.Modified:
                            ModifiedFilePaths.Add(filePath);
                            break;
                        case ChangeKind.Renamed:
                            RenamedFilePaths.Add(filePath);
                            break;
                        case ChangeKind.Copied:
                            break;
                        case ChangeKind.Ignored:
                            break;
                        case ChangeKind.Conflicted:
                            ConflictedFilePaths.Add(filePath);
                            break;
                        default:
                            continue;
                    }
                }
            }

            return true;
        }
    }
}

#endif