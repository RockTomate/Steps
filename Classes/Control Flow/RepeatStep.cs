using System.Collections;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Enums;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Coroutines;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Repeat", "Run child steps specified number of times.", StepCategories.ControlFlowCategory)]
    public class RepeatStep : Step
    {
        [InputField(tooltip: "Number of times to repeat sub-steps.")]
        public int RepeatCount = 5;

        [InputField(tooltip: "Name of the variable which will be resolved as current repeat count.", required: true)]
        public string RepeatNumberName = "i";

        [InputField(tooltip: "Whether or not to start count at zero (the repeat count will offset as well).")]
        public bool StartAtZero = false;

        [InputField(tooltip: "Supplied value will be decremented instead of incremented.")]
        public bool Reverse = false;

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

        /// <inheritdoc />
        /// <summary>
        /// Gets the text that would specify the current status of this step.
        /// </summary>
        public override string InProgressText
        {
            get
            {
                return RepeatCount == 0
                    ? "In Progress"
                    : string.Format("{0:N0}/{1:N0}", CurrentIteration, RepeatCount);
            }
        }

        /// <summary>
        /// Resets the loop.
        /// </summary>
        protected override void OnReset()
        {
            CurrentIteration = 1;
            DoLoop = true;
            InnerScope.Reset();
        }

        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            InnerScope.ScopeContext = new JobContext(context);

            if (InnerScope.Steps.Count == 0)
            {
                IsSuccess = true;
                yield break;
            }

            var startCount = StartAtZero ? 0 : 1;
            var endCount = StartAtZero
                ? RepeatCount - 1
                : RepeatCount;

            if (!Reverse)
            {
                for (var i = startCount; i <= endCount && DoLoop; i++)
                    yield return EditorCoroutines.StartCoroutine(OnIteration(i), this);
            }
            else
            {
                for (var i = endCount; i >= startCount && DoLoop; i--)
                    yield return EditorCoroutines.StartCoroutine(OnIteration(i), this);
            }

            IsSuccess = true;
            yield return null;
        }

        private IEnumerator OnIteration(int index)
        {
            RockLog.InsertLine();
            RockLog.WriteLine(LogTier.Debug, string.Format("Starting iteration number: {0}", CurrentIteration));

            InnerScope.Reset();

            var iteratorVariable = Field<object>.Create(RepeatNumberName, index, false, true);

            // update the iterator job context
            InnerScope.ScopeContext.Add(iteratorVariable);

            // reset the inner scope and loop through the child steps
            yield return EditorCoroutines.StartCoroutine(InnerScope.StartExecutingSteps(), this);

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

        /// <inheritdoc />
        protected override string Description
        {
            get { return string.Format("Run child steps {0:N0} times", RepeatCount); }
        }
    }
}