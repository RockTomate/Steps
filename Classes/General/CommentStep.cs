using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Extensions;
using HardCodeLab.RockTomate.Core.Preferences;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Comment", "Places a comment")]
    public class CommentStep : NeutralStep
    {
        private const string CommentPlaceholderText = "- THIS IS A COMMENT. MODIFY \"NOTES\" OF THIS STEP TO OVERRIDE THIS TEXT -"; 
        
        /// <inheritdoc />
        public override string ToString()
        {
            if (Notes.IsNullOrWhiteSpace())
                return CommentPlaceholderText;

            if (RTPreferences.Data.CommentsAreUppercase)
                return Notes.ToUpper();

            return Notes;
        }
    }
}