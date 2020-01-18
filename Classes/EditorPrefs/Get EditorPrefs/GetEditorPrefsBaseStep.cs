using UnityEditor;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    public abstract class GetEditorPrefsBaseStep<T> : SimpleStep
    {
        public const string PrefsDescription = "Gets the value of preference by identified key.";

        [InputField(tooltip: "Key which identifies an editor pref item.", required: true)]
        public string Key;

        [OutputField(tooltip: "Value which is returned from editor prefs.")]
        public T ReturnValue;

        /// <inheritdoc />
        protected override bool OnValidate()
        {
            return EditorPrefs.HasKey(Key);
        }

        /// <inheritdoc />
        protected override string Description
        {
            get { return string.Format("Returns the value of type {0} corresponding to key \"{1}\" in the preference file if it exists.", typeof(T).Name, Key); }
        }
    }
}