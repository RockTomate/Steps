using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HardCodeLab.RockTomate.Steps;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Metadata;
using HardCodeLab.RockTomate.Editor.Popups;
using HardCodeLab.RockTomate.Editor.Managers;
using HardCodeLab.RockTomate.Core.Extensions;
using HardCodeLab.RockTomate.Editor.Attributes;

namespace HardCodeLab.RockTomate.Editor.Controls
{
    [StepDrawerTarget(typeof(RunJobStep))]
    public class RunJobStepDrawer : StepDrawer
    {
        private static Rect _addVarButtonRect;
        private static readonly List<string> KeysToRemove = new List<string>();

        /// <inheritdoc />
        protected override void RenderInputFields(Step step, StepMetadata stepMetadata)
        {
            var runJobStep = (RunJobStep)step;
            base.RenderInputFields(runJobStep, stepMetadata);
            EditorGUILayout.Space();

            if (runJobStep.TargetJob == null)
            {
                EditorGUILayout.HelpBox("\"Target Job\" field is empty", MessageType.Warning);
                return;
            }

            if (runJobStep.TargetJob == JobTracker.TargetJob)
            {
                EditorGUILayout.HelpBox("\"Target Job\" cannot be the same as a running job!", MessageType.Error);
                return;
            }

            RenderExistingVariableFields(runJobStep);
            EditorGUILayout.Space();
            RenderAdditionalVariableFields(runJobStep);

            RemovePendingKeys(runJobStep);
        }

        private static void RemovePendingKeys(RunJobStep runJobStep)
        {
            if (KeysToRemove.Count == 0)
                return;

            foreach (var key in KeysToRemove)
            {
                if (runJobStep.TargetJobVariables.Remove(key))
                    continue;

                runJobStep.NewVariables.Remove(key);
            }

            KeysToRemove.Clear();
        }

        private static void RenderAdditionalVariableFields(RunJobStep runJobStep)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Additional Variables", EditorStyles.boldLabel);

            if (GUILayout.Button(new GUIContent("Add", "Adds an additional variable"),
                EditorStyles.miniButton, GUILayout.Width(35)))
                ShowAddVariableDialog(runJobStep);

            _addVarButtonRect = GUILayoutUtility.GetLastRect();

            EditorGUILayout.EndHorizontal();

            // render variables
            if (runJobStep.NewVariables.Count == 0)
            {
                EditorGUILayout.HelpBox("Nothing added. You can create additional variables for a job that you wish to run.", MessageType.Info);
                return;
            }

            foreach (var keyValuePair in runJobStep.NewVariables)
            {
                EditorGUILayout.BeginHorizontal();
                RenderVariableField(keyValuePair.Value);

                if (GUILayout.Button(new GUIContent("-", "Remove variable field"), EditorStyles.miniButton, GUILayout.Width(16)))
                    KeysToRemove.Add(keyValuePair.Key);

                EditorGUILayout.EndHorizontal();
            }
        }

        private static void RenderExistingVariableFields(RunJobStep runJobStep)
        {
            var targetJob = runJobStep.TargetJob;
            var formulaEnabled = runJobStep.Formulas["TargetJob"].MainFormula.Enabled;

            if (formulaEnabled)
                return;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Job Variables", EditorStyles.boldLabel);

            if (GUILayout.Button(new GUIContent("Add", "Adds an existing variable to the TargetJob. This option is unavailable for a formula-resolved job."),
                EditorStyles.miniButton, GUILayout.Width(35)))
                ShowAddExistingVariableMenu(runJobStep);

            _addVarButtonRect = GUILayoutUtility.GetLastRect();

            EditorGUILayout.EndHorizontal();

            if (targetJob.Variables.Count == 0)
            {
                EditorGUILayout.HelpBox("This job has no variables", MessageType.Info);
                return;
            }

            // render variables
            if (runJobStep.TargetJobVariables.Count == 0)
            {
                EditorGUILayout.HelpBox("Nothing added. Add job variables that you wish to modify before starting the job.", MessageType.Info);
                return;
            }

            foreach (var keyValuePair in runJobStep.TargetJobVariables)
            {
                EditorGUILayout.BeginHorizontal();
                RenderVariableField(keyValuePair.Value);

                if (GUILayout.Button(new GUIContent("-", "Remove variable field"), EditorStyles.miniButton, GUILayout.Width(16)))
                    KeysToRemove.Add(keyValuePair.Key);

                EditorGUILayout.EndHorizontal();
            }
        }

        private static void ShowAddVariableDialog(RunJobStep runJobStep)
        {
            var formulaEnabled = runJobStep.Formulas["TargetJob"].MainFormula.Enabled;

            var newVariablePopup = new VariableCreationPopup(varName =>
            {
                if (formulaEnabled)
                    return true;

                return !runJobStep.TargetJob.Variables.Any(x => x.Name.Equals(varName));
            });

            newVariablePopup.VariableCreated += variable =>
            {
                runJobStep.NewVariables.Add(variable.Name, variable);
            };

            PopupWindow.Show(_addVarButtonRect, newVariablePopup);
        }

        private static void ShowAddExistingVariableMenu(RunJobStep runJobStep)
        {
            var menu = new GenericMenu();
            var targetJob = runJobStep.TargetJob;

            foreach (var jobVariable in targetJob.Variables)
            {
                if (!runJobStep.TargetJobVariables.ContainsKey(jobVariable.Name))
                {
                    menu.AddItem(new GUIContent(jobVariable.Name), false, () =>
                    {
                        var newVariableField = Field<object>.Create(jobVariable.Name,
                            jobVariable.ReturnType.GetDefaultValueType(), jobVariable.ReturnType);

                        runJobStep.TargetJobVariables.Add(jobVariable.Name, newVariableField);
                    });
                }
            }

            menu.ShowAsContext();
        }
    }
}