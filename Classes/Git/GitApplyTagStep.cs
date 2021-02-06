﻿using LibGit2Sharp;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Apply Tag", "Applies a tag on latest commit on current branch.", StepCategories.GitCategory)]
    public class GitApplyTagStep : BaseGitStep
    {
        [InputField(required: true)]
        public string TagName;

        protected override bool OnStepStart()
        {
            using (var repo = GetRepository())
            {
                repo.ApplyTag(TagName);
            }

            return true;
        }
    }
}