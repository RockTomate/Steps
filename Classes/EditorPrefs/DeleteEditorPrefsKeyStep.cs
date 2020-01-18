using UnityEditor;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Delete EditorPrefs", "Deletes an EditorPrefs Key", StepCategories.EditorPrefsCategory)]
    public class DeleteEditorPrefsKeyStep : SimpleStep
    {
        [InputField(tooltip: "Key which identifies an editor pref item.", required: true)]
        public string Key;

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            return EditorPrefs.HasKey(Key);
        }

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            EditorPrefs.DeleteKey(Key);
            return true;
        }
    }
}