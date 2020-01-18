using UnityEditor;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Set EditorPrefs - Integer", PrefsDescription, StepCategories.EditorPrefsCategory)]
    public class SetEditorPrefsIntStep : SetEditorPrefsBaseStep<int>
    {
        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            EditorPrefs.SetInt(Key, NewValue);
            return true;
        }
    }
}