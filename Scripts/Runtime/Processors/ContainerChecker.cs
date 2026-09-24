using System;

using Core;

using Unity.Entities;

using UnityEngine;

namespace Input
{
    [CreateAssetMenu(fileName = "Container Checker", menuName = "Input/Processors/Container Checker")]
    public class ContainerChecker : Processor
    {
        public override string JSONPath => "Container Checkers/";
        public override Command Command => command;

        [Space]
        public ContainerCheckerCommand command;
    }

    [Serializable]
    public class ContainerCheckerCommand : Command
    {
        [Space]
        [LogInfo] public string[] Keys;

        protected override bool Invoke(string data)
        {
            var contains = true;
            var arr = data.Split();
            for (int k = 0; k < Keys.Length; k++)
            {
                contains &= Contains(Keys[k].ToLower());
                if (!contains)
                    break;
            }

            if (contains)
                Sys.Add_M(Input, World.DefaultGameObjectInjectionWorld.EntityManager);

            return contains;

            bool Contains(string key)
            {
                for (int a = 0; a < arr.Length; a++)
                    if (arr[a].ToLower().Equals(key))
                        return true;

                return false;
            }
        }
    }
}