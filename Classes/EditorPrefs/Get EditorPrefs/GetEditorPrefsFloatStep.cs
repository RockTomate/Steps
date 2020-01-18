using UnityEditor;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Get EditorPrefs - Float", PrefsDescription, StepCategories.EditorPrefsCategory)]
    public class GetEditorPrefsFloatStep : GetEditorPrefsBaseStep<float>
    {
        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            ReturnValue = EditorPrefs.GetFloat(Key);
            return true;
        }
    }
}