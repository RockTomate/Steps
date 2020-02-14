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
    /// Simulates a 
    /// </summary>
    /// <seealso cref="T:HardCodeLab.RockTomate.Core.Steps.Step" />
    [StepDescription("WHILE", "Iterates through each child action while condition is true.", StepCategories.ControlFlowCategory)]
    public class WhileStep : Step
    {
        [InputField("Conditions", "Conditions that need to be fulfilled to execute child Steps.", FieldMode.NoFormula)]
        public ConditionCollection ConditionCollection = new ConditionCollection();

        [InputField(tooltip: "Name of a variable which will represent the current iteration of the loop.")]
        public string IndexCountName = "index";

        /// <summary>
        /// Number of times child steps were repeated.
        /// </summary>
        private int CurrentIteration
        {
            get { return RuntimeState.Get("current_count", 1); }
            set { RuntimeState.Set("current_count", value); }
        }

        private bool DoLoop
        {
            get { return RuntimeState.Get("do_loop", true); }
            set { RuntimeState.Set("do_loop", value); }
        }

        /// <inheritdoc />
        /// <summary>
        /// Indicates that this Step can accept children.
        /// </summary>
        public override bool AllowChildren
        {
            get { return true; }
        }

        public delegate void LoopDelegate(int iterationNumber);

        /// <summary>
        /// Invoked when a loop iteration has begun.
        /// </summary>
        public event LoopDelegate IterationStart;

        /// <summary>
        /// Invoked when loop iteration ended.
        /// </summary>
        public event LoopDelegate IterationEnd;

        /// <summary>
        /// Resets the loop.
        /// </summary>
        protected override void OnReset()
        {
            DoLoop = true;
            CurrentIteration = 0;
            InnerScope.Reset();
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when this Step gets executed.
        /// </summary>
        /// <param name="context">Current job context.</param>
        /// <returns></returns>
        protected override IEnumerator OnExecute(JobContext context)
        {
            InnerScope.ScopeContext = new JobContext(context);

            if (InnerScope.Steps.Count == 0)
            {
                IsSuccess = true;
                yield break;
            }

            while (ConditionCollection.EvaluateAll(context))
            {
                if (!DoLoop)
                    break;

                OnIterationStart(CurrentIteration);
                InnerScope.Reset();

                var indexCountVariable = Field<object>.Create(IndexCountName.Trim('%'), CurrentIteration, false, true);

                // update the iterator job context
                InnerScope.ScopeContext.Add(indexCountVariable);

                // reset the inner scope and loop through the child steps
                yield return EditorCoroutines.StartCoroutine(InnerScope.StartExecutingSteps(), this);

                OnIterationEnd(CurrentIteration);

                if (!IsSuppressed)
                {
                    switch (InnerScope.Request)
                    {
                        case StepRequest.ExitScope:
                        case StepRequest.RootScope:
                        case StepRequest.TerminateJob:

                            DoLoop = false;

                            break;
                    }
                }

                CurrentIteration++;
            }

            IsSuccess = true;
            yield return null;
        }

        protected virtual void OnIterationStart(int iterationNumber)
        {
            if (IterationStart != null)
                IterationStart(iterationNumber);
        }

        protected virtual void OnIterationEnd(int iterationNumber)
        {
            if (IterationEnd != null)
                IterationEnd(iterationNumber);
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the text that would specify the current status of this step.
        /// </summary>
        public override string InProgressText
        {
            get
            {
                return string.Format("Iteration {0:N0}", CurrentIteration + 1);
            }
        }

        /// <inheritdoc />
        protected override string Description
        {
            get { return string.Format("Iterate through {0:N0} children, while ({1})", InnerScope.Steps.Count, ConditionCollection); }
        }
    }
}