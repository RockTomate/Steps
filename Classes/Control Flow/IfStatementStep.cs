using System;
using System.Collections;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Enums;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Coroutines;

namespace HardCodeLab.RockTomate.Steps
{
    /// <inheritdoc />
    /// <summary>
    /// A decision which has a simple branching between true and false.
    /// </summary>
    /// <seealso cref="T:HardCodeLab.RockTomate.Core.Steps.Decision" />
    [StepDescription("IF", "If conditions are true, then child Steps are executed.", StepCategories.ControlFlowCategory)]
    public sealed class IfStatementStep : Step
    {
        /// <summary>
        /// A set of conditions that will be evaluated.
        /// </summary>
        [InputField("Conditions", "Conditions that need to be fulfilled to execute child Steps.", FieldMode.NoFormula)]
        public ConditionCollection ConditionCollection = new ConditionCollection();

        public IfStatementStep()
        {
        }

        public IfStatementStep(params Condition[] conditions) : this()
        {
            ConditionCollection.Conditions.AddRange(conditions);
        }

        /// <inheritdoc />
        protected override void OnReset()
        {
            InnerScope.Reset();
        }

        /// <inheritdoc />
        /// <summary>
        /// Executes this step.
        /// </summary>
        /// <param name="context">Current job context.</param>
        /// <returns>Returns true if execution was successful.</returns>
        protected override IEnumerator OnExecute(JobContext context)
        {
            var isConditionTrue = ConditionCollection.EvaluateAll(context);

            if (isConditionTrue)
            {
                InnerScope.ScopeContext = context;
                yield return EditorCoroutines.StartCoroutine(InnerScope.StartExecutingSteps(), this);
                PostExecutionRequest = StepRequest.SkipNext;
            }
            else
            {
                foreach (var child in Children)
                {
                    child.Status = StepStatus.Skipped;
                    yield return null;
                }
            }

            IsSuccess = true;
            yield return null;
        }

        /// <inheritdoc />
        public override bool AllowChildren
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override Type SkipNextStepType
        {
            get { return typeof(ElseStatementStep); }
        }

        /// <inheritdoc />
        protected override string Description
        {
            get
            {
                if (ConditionCollection.IsEmpty)
                    return "Conditions are true";

                return string.Format("Conditions are true ({0})", ConditionCollection);
            }
        }
    }
}