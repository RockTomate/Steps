using System.Collections;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Enums;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Set Variable", "Updates the value of a variable")]
    public class SetVariableStep : Step
    {
        [InputField(tooltip: "Name of the variable which will be affected. Variable identifiers ('%') will be automatically stripped.", required: true)]
        public string VariableName;

        [InputField(fieldMode: FieldMode.FormulaOnly, required: true)]
        public object NewValue;

        [InputField(tooltip: "If true, a new variable will be created with specified value instead of failing the step.")]
        public bool CreateNew = false;

        /// <inheritdoc />
        protected override IEnumerator OnExecute(JobContext context)
        {
            BaseField variable;
            if (!context.TryGetField(VariableName, out variable))
            {
                if (CreateNew)
                {
                    variable = Field<string>.Create(VariableName, NewValue);
                    context.Add(variable);
                }
                else
                {
                    IsSuccess = false;
                    RockLog.WriteLine(this, LogTier.Error, string.Format("Couldn't find variable of name: {0}", VariableName));
                    yield return null;
                }
            }

            variable.SetValue(NewValue);

            IsSuccess = true;
            yield return null;
        }
    }
}