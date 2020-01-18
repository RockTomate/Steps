using UnityEditor;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Set EditorPrefs - Boolean", PrefsDescription, StepCategories.EditorPrefsCategory)]
    public class SetEditorPrefsBoolStep : SetEditorPrefsBaseStep<bool>
    {
        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            EditorPrefs.SetBool(Key, NewValue);
            return true;
        }
    }
}