using UnityEditor;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Set EditorPrefs - String", PrefsDescription, StepCategories.EditorPrefsCategory)]
    public class SetEditorPrefsStringStep : SetEditorPrefsBaseStep<string>
    {
        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            EditorPrefs.SetString(Key, NewValue);
            return true;
        }
    }
}