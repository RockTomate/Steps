﻿using System;
using UnityEngine;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Logging;
using HardCodeLab.RockTomate.Core.Attributes;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Create GameObject", "Creates a GameObject in a currently active Scene.", StepCategories.ScenesCategory)]
    public class CreateGameObjectStep : SimpleStep
    {
        [InputField]
        public string Name = "GameObject";

        [InputField]
        public string Tag = "Untagged";

        [InputField(tooltip: "Position where new GameObject will be placed")]
        public Vector3 Position = Vector3.zero;

        [InputField(tooltip: "Components that you want this GameObject to have")]
        public string[] Components;

        [OutputField(tooltip: "Created GameObject")]
        public GameObject GameObject;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            GameObject = new GameObject(Name)
            {
                tag = Tag
            };

            GameObject.transform.position = Position;

            foreach (var compName in Components)
            {
                var componentType = Type.GetType(compName);
                if (componentType == null)
                {
                    RockLog.WriteLine(this, LogTier.Error, string.Format("Component by name \"{0}\" couldn't be added to GameObject named \"{1}\". Skipping...", compName, Name));
                    continue;
                }

                GameObject.AddComponent(componentType);
            }

            return true;
        }
    }
}