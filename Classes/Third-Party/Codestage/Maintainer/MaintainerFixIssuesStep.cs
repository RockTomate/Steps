using System.IO;
using UnityEngine;
using CodeStage.Maintainer;
using CodeStage.Maintainer.Issues;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Enums;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Extensions;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Fix Issues", "Fixes issues found by the Maintainer", "\"Maintainer\" Plugin")]
    public class MaintainerFixIssuesStep : SimpleStep
    {
        private const string ReportCategoryName = "Reporting";

        [InputField(tooltip: "Issues that will be attempted to be fixed.", fieldMode: FieldMode.FormulaOnly)]
        public IssueRecord[] Issues;

        [InputField(tooltip: "Fail this step if there are any unfixed issues left.")]
        public bool FailOnIssues = true;

        [InputField(tooltip: "If true, all fixed issues will be printed to the console.", category: ReportCategoryName)]
        public bool PrintFixedIssues = false;

        [InputField(tooltip: "If true, a report on fixed issues will be generated and saved as a text file.", category: ReportCategoryName)]
        public bool SaveReport = false;

        [InputField(tooltip: "File path where report would be saved (used if \"SaveReport\" is enabled).", category: ReportCategoryName)]
        public string ReportFilePath = string.Empty;

        [OutputField(tooltip: "Issues that have been fixed")]
        public IssueRecord[] FixedIssues;

        protected override bool OnStepStart()
        {
            FixedIssues = IssuesFinder.StartFix(Issues, false, false);

            if (SaveReport)
            {
                var report = ReportsBuilder.GenerateReport("Fixed Issues Report (executed through RockTomate)", FixedIssues);
                File.WriteAllText(ReportFilePath, report);
            }
            
            if (PrintFixedIssues)
            {
                FixedIssues.ForEach(Debug.Log);
            }

            return !FailOnIssues || FixedIssues.Length >= Issues.Length;
        }
    }
}