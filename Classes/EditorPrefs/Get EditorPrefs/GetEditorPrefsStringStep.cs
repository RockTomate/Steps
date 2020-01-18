using UnityEditor;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Get EditorPrefs - String", PrefsDescription, StepCategories.EditorPrefsCategory)]
    public class GetEditorPrefsStringStep : GetEditorPrefsBaseStep<string>
    {
        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            ReturnValue = EditorPrefs.GetString(Key);
            return true;
        }
    }
}