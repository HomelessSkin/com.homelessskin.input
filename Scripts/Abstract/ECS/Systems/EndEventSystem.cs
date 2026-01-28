using Core;

using Unity.Entities;

namespace Input
{
    public abstract partial class EndEventSystem : BehaviourSystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            RequireForUpdate<OuterInput.End>();
        }
        protected override void Proceed()
        {
            EntityManager.DestroyEntity(EntityManager.CreateEntityQuery(typeof(OuterInput.End)));
        }
    }
}