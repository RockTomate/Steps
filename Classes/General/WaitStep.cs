using System;
using System.Collections;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Wait", "Waits for a specified amount of time.")]
    public class WaitStep : Step
    {
        private float _currentTimeLeftMs;
        private DateTime _prevTimeSinceStartup;

        [InputField(tooltip: "Number of milliseconds to wait.")]
        public int Duration = 1000;

        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            _currentTimeLeftMs = Duration;
            _prevTimeSinceStartup = DateTime.Now;

            while (_currentTimeLeftMs > 0.0f)
            {
                float deltaTime = (float)DateTime.Now.Subtract(_prevTimeSinceStartup).TotalMilliseconds;
                _prevTimeSinceStartup = DateTime.Now;

                _currentTimeLeftMs -= deltaTime;

                yield return null;
            }

            IsSuccess = true;
        }

        /// <inheritdoc />
        public override string InProgressText
        {
            get { return "Waiting"; }
        }

        /// <inheritdoc />
        protected override string Description
        {
            get { return string.Format("Wait {0:N0} milliseconds", Duration); }
        }
    }
}