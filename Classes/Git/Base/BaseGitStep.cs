#if NET_4_6 || NET_STANDARD_2_0

using LibGit2Sharp;
using HardCodeLab.RockTomate.Core.Enums;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    public abstract class BaseGitStep : SimpleStep
    {
        protected const string ConfigCategory = "Config";

        [InputField("Repository", "Repository resource.\nUse \"Git - Get Repository\" step to get this resource.", FieldMode.FormulaOnly, true, ConfigCategory)]
        public IRepository Repository;
    }
}

#endif