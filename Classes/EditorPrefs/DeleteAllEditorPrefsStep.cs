using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Steps;
using UnityEditor;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Delete All EditorPrefs", "Deletes all EditorPrefs keys.", StepCategories.EditorPrefsCategory)]
    public class DeleteAllEditorPrefsStep : SimpleStep
    {
        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            EditorPrefs.DeleteAll();
            return true;
        }
    }
}