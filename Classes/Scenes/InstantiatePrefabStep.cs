using UnityEngine;
using HardCodeLab.RockTomate.Core.Data;
using HardCodeLab.RockTomate.Core.Steps;
using HardCodeLab.RockTomate.Core.Attributes;
using HardCodeLab.RockTomate.Core.Extensions;

namespace HardCodeLab.RockTomate.Steps
{
    [StepDescription("Instantiate Prefab", "Loads a prefab and instantiates it into a currently active Scene.")]
    public class InstantiatePrefabStep : SimpleStep
    {
        [InputField(tooltip: "Prefab that will be instantiated.")]
        public UnityGameObject Prefab;

        [InputField(tooltip: "Starting position of the instantiated GameObject.")]
        public Vector3 Position;

        [InputField(tooltip: "Name of the instantiated GameObject (if it's empty then the name won't be altered).")]
        public string Name;

        [OutputField(name: "GameObject", tooltip: "Instantiated GameObject")]
        public GameObject InstantiatedGameObject;

        /// <inheritdoc />
        protected override bool OnStepStart()
        {
            InstantiatedGameObject = Object.Instantiate(Prefab.Value, Position, Quaternion.identity);
            InstantiatedGameObject.name = !Name.IsNullOrWhiteSpace() 
                ? Name 
                : Prefab.Value.name;

            return true;
        }
    }
}