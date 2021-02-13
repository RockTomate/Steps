#if NET_4_6 || NET_STANDARD_2_0

using System;
using LibGit2Sharp;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Git - Create Branch", "Creates a new branch.", StepCategories.GitCategory)]
    public class GitCreateBranchStep : BaseGitStep
    {
        [InputField(tooltip: "Name of the new branch.", required: true)]
        public string NewBranchName;

        [InputField(tooltip: nameof(CheckoutBehaviourTypes.OnCreateOnly) + " - will checkout to new branch only if it has been successfully created\n" +
                             nameof(CheckoutBehaviourTypes.Always) + " - will checkout to specified branch even if it already existed before\n" +
                             nameof(CheckoutBehaviourTypes.Never) + " - will never checkout to new branch")]
        public CheckoutBehaviourTypes CheckoutBehaviour = CheckoutBehaviourTypes.OnCreateOnly;

        [OutputField(tooltip: "Whether new branch has been created after this step.")]
        public bool BranchCreated;

        protected override bool OnStepStart()
        {
            using (var repo = GetRepository())
            {
                var branch = repo.Branches[NewBranchName];

                if (branch == null)
                {
                    branch = repo.CreateBranch(NewBranchName);
                    BranchCreated = true;
                }
                else
                {
                    RockLog.WriteLine(this, LogTier.Warning, $"Branch with the name \"{NewBranchName}\" already exists.");
                }

                switch (CheckoutBehaviour)
                {
                    case CheckoutBehaviourTypes.OnCreateOnly:

                        if (BranchCreated)
                        {
                            CheckoutBranch(repo, branch);
                        }
                        break;

                    case CheckoutBehaviourTypes.Always:
                        CheckoutBranch(repo, branch);
                        break;

                    case CheckoutBehaviourTypes.Never:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return true;
        }

        private void CheckoutBranch(IRepository repo, Branch branch)
        {
            RockLog.WriteLine(this, LogTier.Info, "Checking out to new branch...");
            Commands.Checkout(repo, branch);
        }

        public enum CheckoutBehaviourTypes
        {
            OnCreateOnly,
            Always,
            Never
        }
    }
}

#endif