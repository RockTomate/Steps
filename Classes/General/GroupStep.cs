using System.Collections;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Coroutines;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Logging;

namespace HardCodeLab.RockTomate.Steps
{
    /// <summary>
    /// Used to group steps together
    /// </summary>
    [StepDescription("Group", "Groups steps together")]
    public class GroupStep : Step
    {
        /// <inheritdoc />
        protected override void OnReset()
        {
            InnerScope.Reset();
        }

        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            RockLog.WriteLine(this, LogTier.Debug, string.Format("Entering Group{0}", string.IsNullOrEmpty(Notes) ? string.Empty : string.Format(": {0}", Notes)));
            RockLog.InsertLine();

            InnerScope.ScopeContext = context;
            yield return EditorCoroutines.StartCoroutine(InnerScope.StartExecutingSteps(), this);
            IsSuccess = true;
            yield return null;

            RockLog.WriteLine(this, LogTier.Debug, string.Format("Exiting Group{0}", string.IsNullOrEmpty(Notes) ? string.Empty : string.Format(": {0}", Notes)));
            RockLog.InsertLine();
        }

        /// <inheritdoc />
        public override bool AllowChildren
        {
            get { return true; }
        }
    }
}