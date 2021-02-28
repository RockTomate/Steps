#if NET_4_6 || NET_STANDARD_2_0

using System;
using LibGit2Sharp;
using System.Collections.Generic;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Git - Get Work Directory Files", "Get files from the working directory of the repository.", StepCategories.GitCategory)]
    public class GitGetWorkDirectoryFilesStep : BaseGitStep
    {
        private const string AllCategory = "All";
        private const string AddedCategory = "Added only";
        private const string DeletedCategory = "Deleted only";
        private const string RenamedCategory = "Renamed only";
        private const string ModifiedCategory = "Modified only";
        private const string ConflictedCategory = "Conflicted only";

        [InputField(category: "Settings")]
        public RetrieveRequestType FileRetrieveType;

        [OutputField(tooltip: "Array of file paths that were affected (added, deleted, renamed etc.).", category: AllCategory)]
        public List<string> FilePaths = new List<string>();

        [OutputField(tooltip: "Array of file paths that were added.", category: AddedCategory)]
        public List<string> AddedFilePaths = new List<string>();

        [OutputField(tooltip: "Array of file paths that were deleted.", category: DeletedCategory)]
        public List<string> DeletedFilePaths = new List<string>();

        [OutputField(tooltip: "Array of file paths that were renamed.", category: RenamedCategory)]
        public List<string> RenamedFilePaths = new List<string>();

        [OutputField(tooltip: "Array of file paths which contents were modified.", category: ModifiedCategory)]
        public List<string> ModifiedFilePaths = new List<string>();

        [OutputField(tooltip: "Array of file paths which contents are in merge conflict.", category: ConflictedCategory)]
        public List<string> ConflictedFilePaths = new List<string>();

        protected override bool OnStepStart()
        {
            DiffTargets diffTargets;

            switch (FileRetrieveType)
            {
                case RetrieveRequestType.All:
                    diffTargets = DiffTargets.Index | DiffTargets.WorkingDirectory;
                    break;

                case RetrieveRequestType.StagedOnly:
                    diffTargets = DiffTargets.Index;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var entryChange in Repository.Diff.Compare<TreeChanges>(Repository.Head.Tip.Tree, diffTargets))
            {
                var filePath = entryChange.Path;

                FilePaths.Add(filePath);

                switch (entryChange.Status)
                {
                    case ChangeKind.Added:
                        AddedFilePaths.Add(filePath);
                        break;
                    case ChangeKind.Deleted:
                        DeletedFilePaths.Add(filePath);
                        break;
                    case ChangeKind.Modified:
                        ModifiedFilePaths.Add(filePath);
                        break;
                    case ChangeKind.Renamed:
                        RenamedFilePaths.Add(filePath);
                        break;
                    case ChangeKind.Copied:
                        break;
                    case ChangeKind.Ignored:
                        break;
                    case ChangeKind.Conflicted:
                        ConflictedFilePaths.Add(filePath);
                        break;
                    default:
                        continue;
                }
            }

            return true;
        }

        public enum RetrieveRequestType
        {
            All,
            StagedOnly,
        }
    }
}

#endif