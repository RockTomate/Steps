using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.Maintainer;
using CodeStage.Maintainer.Issues;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Extensions;
using File = System.IO.File;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Run Issues Finder", "Runs Maintainer's \"Issues Finder\"", "\"Maintainer\" Plugin")]
    public class MaintainerRunIssuesFinderStep : SimpleStep
    {
        private const string FiltersCategoryName = "Filtering";
        private const string ReportCategoryName = "Reporting";

        [InputField(tooltip: "Print issues to Console Window.")]
        public bool PrintIssues = false;

        [InputField(tooltip: "Fail this step if there issues have been found.")]
        public bool FailOnIssues = true;

        [InputField(tooltip: "If true, issues of type \"Info\" will be included.", category: FiltersCategoryName)]
        public bool IncludeInfo = true;

        [InputField(tooltip: "If true, issues of type \"Warning\" will be included.", category: FiltersCategoryName)]
        public bool IncludeWarning = true;

        [InputField(tooltip: "If true, issues of type \"Error\" will be included.", category: FiltersCategoryName)]
        public bool IncludeError = true;

        [InputField(tooltip: "If true, a report will be generated and saved as a text file.", category: ReportCategoryName)]
        public bool SaveReport = false;

        [InputField(tooltip: "File path where report would be saved (used if \"SaveReport\" is enabled).", category: ReportCategoryName)]
        public string ReportFilePath = string.Empty;

        [OutputField(tooltip: "Issues that have been found.")]
        public IssueRecord[] Issues;

        [OutputField(tooltip: "Issues that have been found (in printed format).")]
        public string[] IssuesPrinted;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            var allIssues = IssuesFinder.StartSearch(false);

            Issues = GetFilteredIssues(allIssues);
            IssuesPrinted = Issues.Select(x => x.ToString(true)).ToArray();

            if (SaveReport)
            {
                var report = ReportsBuilder.GenerateReport("Found Issues Report (executed through RockTomate)", Issues);
                File.WriteAllText(ReportFilePath, report);
            }

            if (PrintIssues)
            {
                Issues.ForEach(Debug.Log);
            }

            return !FailOnIssues || Issues.Length <= 0;
        }

        /// <summary>
        /// Filters out ignored issues.
        /// </summary>
        /// <param name="issues">Issues that will be filtered.</param>
        /// <returns>Returns an array of filtered issues.</returns>
        private IssueRecord[] GetFilteredIssues(IssueRecord[] issues)
        {
            if (IncludeInfo && IncludeError && IncludeWarning)
                return issues;

            var filteredIssues = new List<IssueRecord>();

            foreach (var issueRecord in issues)
            {
                switch (issueRecord.Severity)
                {
                    case IssueSeverity.Info:

                        if (!IncludeInfo)
                            continue;
                        break;

                    case IssueSeverity.Warning:

                        if (!IncludeWarning)
                            continue;
                        break;

                    case IssueSeverity.Error:

                        if (IncludeError)
                            continue;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                filteredIssues.Add(issueRecord);
            }

            return filteredIssues.ToArray();
        }
    }
}