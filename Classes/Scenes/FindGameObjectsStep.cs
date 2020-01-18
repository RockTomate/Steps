using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Extensions;
using Object = UnityEngine.Object;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Find GameObjects", "Finds GameObjects in all loaded scenes", StepCategories.ScenesCategory)]
    public class FindGameObjectsStep : SimpleStep
    {
        [InputField("GameObject Name")]
        public string GameObjectName;

        [InputField]
        public string TagName;

        [InputField]
        public string[] ContainsComponentsOfName;

        [InputField(tooltip: "Comparison culture which will be used when comparing names")]
        public StringComparison ComparisonCulture = StringComparison.OrdinalIgnoreCase;

        [OutputField]
        [NonSerialized]
        public IEnumerable<GameObject> Result;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            Result = Object.FindObjectsOfType(typeof(GameObject)).Where(@object =>
            {
                var gameObject = (GameObject)@object;

                // filter by name
                if (!string.IsNullOrEmpty(GameObjectName) && !gameObject.name.Contains(GameObjectName, ComparisonCulture))
                    return false;

                // filter by tag
                if (!string.IsNullOrEmpty(TagName) && !gameObject.tag.Contains(TagName, ComparisonCulture))
                    return false;

                // filter by components contained within
                if (ContainsComponentsOfName.Length > 0)
                {
                    if (!gameObject.GetComponents<Component>().Any(component => ContainsComponentsOfName
                        .Any(compName => compName.Contains(component.name, ComparisonCulture))))
                    {
                        return false;
                    }
                }

                return true;
            })
            .Cast<GameObject>();

            return true;
        }
    }
}