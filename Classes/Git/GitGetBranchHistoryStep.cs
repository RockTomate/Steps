#if NET_4_6 || NET_STANDARD_2_0

using System;
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
        public IEnumerable<VCCommit> CommitHistory;

        protected override bool OnStepStart()
        {
            var commits = CommitCount > 0
                ? Repository.Commits.Take(CommitCount)
                : Repository.Commits;

            CommitHistory = commits.Select(VCCommit.Create);

            return true;
        }

        /// <summary>
        /// Stores light data on a commit.
        /// Reason why we don't use <see cref="Commit"/> instance directly is because most data in there is lazy-loaded.
        /// By the time we would need to access its data, the <see cref="Repository"/> instance would already be disposed of, leading to crashes.
        /// </summary>
        [Serializable]
        public struct VCCommit
        {
            /// <summary>
            /// Returns an instance of a VC Commit.
            /// </summary>
            /// <param name="commit">Commit instance</param>
            /// <returns><see cref="VCCommit"/> instance.</returns>
            public static VCCommit Create(Commit commit)
            {
                return new VCCommit
                {
                    Author = commit.Author,
                    Committer = commit.Committer,
                    Sha = commit.Sha,
                    Message = commit.Message,
                };
            }

            public string Sha { get; private set; }
            public string Message { get; private set; }
            public Signature Author { get; set; }
            public Signature Committer { get; private set; }
        }
    }
}

#endif