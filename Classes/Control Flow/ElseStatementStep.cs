using System.Collections;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Coroutines;

namespace HardCodeLab.RockTomate.Steps
{
    /// <inheritdoc />
    /// <summary>
    /// Gets executed only if the previous <see cref="T:HardCodeLab.RockTomate.Core.Steps.IfStatementStep" /> Step has evaluated to false. 
    /// Otherwise, it gets executed as normal.
    /// </summary>
    /// <seealso cref="T:HardCodeLab.RockTomate.Core.Steps.Step" />
    [StepDescription("ELSE", "Executes child steps if prior condition returned false.", StepCategories.ControlFlowCategory)]
    public class ElseStatementStep : Step
    {
        /// <inheritdoc />
        protected override void OnReset()
        {
            InnerScope.Reset();
        }

        protected override IEnumerator OnExecute(JobContext context)
        {
            InnerScope.ScopeContext = context;
            yield return EditorCoroutines.StartCoroutine(InnerScope.StartExecutingSteps(), this);
            IsSuccess = true;
            yield return null;
        }

        /// <inheritdoc />
        public override bool AllowChildren
        {
            get { return true; }
        }
    }
}