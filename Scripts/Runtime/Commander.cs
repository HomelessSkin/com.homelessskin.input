#if UNITY_EDITOR
using Core;

using Unity.Entities;

using UnityEngine;

namespace Input
{
    public class Commander : MonoBehaviour
    {
        [SerializeField] OuterInput Input;

        public void Send() => Sys.Add_M(Input, World.DefaultGameObjectInjectionWorld.EntityManager);
    }
}
#endif