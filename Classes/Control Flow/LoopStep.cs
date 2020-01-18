using System;
using System.Collections;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Enums;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Coroutines;
using HardCodeLab.RockTomate.Core.Extensions;

namespace HardCodeLab.RockTomate.Steps
{
    /// <inheritdoc />
    /// <summary>
    /// Step that allows to loop through steps.
    /// </summary>
    /// <seealso cref="T:HardCodeLab.RockTomate.Core.Steps.Step" />
    [StepDescription("Loop", "Iterates through each child action.", StepCategories.ControlFlowCategory)]
    public class LoopStep : Step
    {
        /// <summary>
        /// The enumerable through which this will iterate through
        /// </summary>
        [NonSerialized]
        [InputField(fieldMode: FieldMode.FormulaOnly, required: true)]
        public IEnumerable ItemList;

        /// <summary>
        /// The iterator name that will be used by formula parsers.
        /// </summary>
        [InputField(tooltip: "Name of the variable which will represent the current item of the loop.", required: true)]
        public string IteratorName = "item";

        /// <summary>
        /// The index name that will be used by formula parsers
        /// </summary>
        [InputField(tooltip: "Name of a variable which will represent the current iteration of the loop.")]
        public string IndexCountName = "index";

        private int TotalItemsCount
        {
            get { return RuntimeState.Get("total_items_count", 0); }
            set { RuntimeState.Set("total_items_count", value); }
        }

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
            TotalItemsCount = 0;
            if (ItemList != null)
                TotalItemsCount = ItemList.Count();

            CurrentIteration = 0;
            InnerScope.Reset();
        }

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            if (ItemList == null)
                return false;

            return true;
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

            foreach (var item in ItemList)
            {
                if (!DoLoop)
                    break;

                OnIterationStart(CurrentIteration);
                InnerScope.Reset();

                var iteratorVariable = Field<object>.Create(IteratorName.Trim('%'), item, false, true);
                var indexCountVariable = Field<object>.Create(IndexCountName.Trim('%'), CurrentIteration, false, true);

                // update the iterator job context
                InnerScope.ScopeContext.Add(iteratorVariable);
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
                return TotalItemsCount == 0
                    ? "In Progress"
                    : string.Format("{0:N0}/{1:N0}", CurrentIteration + 1, TotalItemsCount);
            }
        }

        /// <inheritdoc />
        protected override string Description
        {
            get { return string.Format("Iterate through {0:N0} children, passing %{1}% as a variable.", InnerScope.Steps.Count, IteratorName); }
        }
    }
}