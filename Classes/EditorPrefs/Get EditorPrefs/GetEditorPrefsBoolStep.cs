using UnityEditor;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Get EditorPrefs - Boolean", PrefsDescription, StepCategories.EditorPrefsCategory)]
    public class GetEditorPrefsBoolStep : GetEditorPrefsBaseStep<bool>
    {
        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            ReturnValue = EditorPrefs.GetBool(Key);
            return true;
        }
    }
}