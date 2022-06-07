using HardCodeLab.RockTomate.Core.Metadata;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Editor.Attributes;
using HardCodeLab.RockTomate.Steps;

namespace HardCodeLab.RockTomate.Editor.Controls
{
    [StepDrawerTarget(typeof(CommentStep))]
    public class CommentStepDrawer : StepDrawer
    {
        /// <inheritdoc />
        protected override void RenderInputFields(Step step, StepMetadata stepMetadata)
        {
            // do not render this section
        }

        /// <inheritdoc />
        protected override void RenderOutputFields(Step step, StepMetadata stepMetadata)
        {
            // do not render this section
        }

        /// <inheritdoc />
        protected override void RenderFieldTypeToolbar(StepMetadata stepMetadata)
        {
            // do not render this section
        }
    }
}