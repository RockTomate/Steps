using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    public abstract class SetEditorPrefsBaseStep<T> : SimpleStep
    {
        public const string PrefsDescription = "Sets the value of preference by identified key.";

        [InputField(tooltip: "Key which identifies an editor pref item.", required: true)]
        public string Key;

        [InputField(tooltip: "New Value of EditorPrefs key.", required: true)]
        public T NewValue;

        /// <inheritdoc />
        protected override string Description
        {
            get { return string.Format("Updates the value of type {0} corresponding to key \"{1}\".", typeof(T).Name, Key); }
        }
    }
}