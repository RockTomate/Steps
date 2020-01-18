using UnityEditor;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Get EditorPrefs - Integer", PrefsDescription, StepCategories.EditorPrefsCategory)]
    public class GetEditorPrefsIntStep : GetEditorPrefsBaseStep<int>
    {
        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            ReturnValue = EditorPrefs.GetInt(Key);
            return true;
        }
    }
}