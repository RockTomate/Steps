using System.Linq;
using LibGit2Sharp;
using HardCodeLab.RockTomate.Core.Extensions;

namespace HardCodeLab.RockTomate.Steps
{
    /// <summary>
    /// Helper class with helper functions
    /// </summary>
    internal static class GitStepsUtils
    {
        /// <summary>
        /// Finds and retrieves Commit by parsing requested search string.
        /// </summary>
        /// <param name="repo">Repository where to search for a commit.</param>
        /// <param name="commitSearch">Commit search string.</param>
        /// <returns>Returns <see cref="Commit"/> instance.</returns>
        internal static Commit GetCommit(IRepository repo, string commitSearch)
        {
            // if user is requesting current commit
            if (commitSearch.IsNullOrWhiteSpace() || commitSearch == "0" || commitSearch == "-0")
            {
                return repo.Head.Tip;
            }

            // if user is requesting relative commit
            if (commitSearch.StartsWith("-"))
            {
                if (!int.TryParse(commitSearch.Substring(1), out var num))
                    return null;

                return repo.Head.Commits.ElementAtOrDefault(num);
            }

            return repo.Lookup<Commit>(commitSearch);
        }
    }
}