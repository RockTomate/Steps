#if NET_4_6 || NET_STANDARD_2_0

using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Git - Stage Files", "Stages files in target repository.", StepCategories.GitCategory)]
    public class GitStageFileStep : BaseGitStep
    {
        [InputField(required: true, tooltip: "Array of paths of files that will be staged.\nFile paths must be relative to the repository directory path (where .git folder is).")]
        public string[] FilesToStage;

        protected override bool OnStepStart()
        {
            using (var repo = GetRepository())
            {
                foreach (var filePath in FilesToStage)
                {
                    repo.Index.Add(filePath);
                }

                repo.Index.Write();
            }

            return true;
        }
    }
}

#endif