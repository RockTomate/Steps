using UnityEditor;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    /// <summary>
    /// Locates a menu item and executes it
    /// </summary>
    /// <seealso cref="SimpleStep" />
    [StepDescription("Invoke Menu Item", "Runs an action from a menu item")]
    public sealed class InvokeMenuItemStep : SimpleStep
    {
        private const string MenuItemPathTip
            = "Path to a Menu Item to execute. " +
              "E.g. if you want to execute \"GameObject > Create Empty\", then the path would be \"GameObject/Create Empty\"";

        [InputField(tooltip: MenuItemPathTip, required: true)]
        public string MenuItemPath;

        [InputField("Hello there!")]
        public bool Test;

        protected override bool OnStepStart()
        {
            return EditorApplication.ExecuteMenuItem(MenuItemPath);
        }
    }
}