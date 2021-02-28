#if NET_4_6 || NET_STANDARD_2_0

using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Git - Add Files", "Stages files in the target repository.", StepCategories.GitCategory)]
    public class GitAddFileStep : BaseGitStep
    {
        [InputField(required: true, tooltip: "Array of paths of files that will be staged.\nFile paths must be relative to the repository directory path (where .git folder is).")]
        public string[] FilesToStage;

        protected override bool OnStepStart()
        {
            foreach (var filePath in FilesToStage)
            {
                Repository.Index.Add(filePath);
            }

            Repository.Index.Write();

            return true;
        }
    }
}

#endif