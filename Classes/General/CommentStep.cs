using System.Collections;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Comment", "Places a comment")]
    public class CommentStep : Step
    {
        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            IsSuccess = true;
            yield return null;
        }

        /// <inheritdoc />
        protected override string Description
        {
            get { return string.Empty; }
        }
    }
}