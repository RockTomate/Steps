#if NET_4_6 || NET_STANDARD_2_0

using System;
using LibGit2Sharp;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Git - Commit", "Commit staged changes to active branch", StepCategories.GitCategory)]
    public class GitCommitStep : BaseGitStep
    {
        [InputField(tooltip: "Commit message", required: true)]
        public string Message;

        protected override bool OnStepStart()
        {
            var timeOffset = DateTimeOffset.UtcNow;
            var signature = Repository.Config.BuildSignature(timeOffset);
            Repository.Commit(Message, signature, signature);

            return true;
        }
    }
}

#endif