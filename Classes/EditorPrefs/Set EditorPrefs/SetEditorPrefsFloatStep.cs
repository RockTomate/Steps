using UnityEditor;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Set EditorPrefs - Float", PrefsDescription, StepCategories.EditorPrefsCategory)]
    public class SetEditorPrefsFloatStep : SetEditorPrefsBaseStep<float>
    {
        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            EditorPrefs.SetFloat(Key, NewValue);
            return true;
        }
    }
}