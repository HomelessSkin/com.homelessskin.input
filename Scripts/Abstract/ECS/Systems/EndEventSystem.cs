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
            var query = EntityManager.CreateEntityQuery(typeof(OuterInput.End));

            var ends = query.ToComponentDataArray<OuterInput.End>();
            for (int e = 0; e < ends.Length; e++)
            {
                var end = ends[e];
                switch (end.Result)
                {
                    case LogLevel.Warning:
                    Log.Warning(this, $"{end.Input.Title} Event was not ended correctly!");
                    break;
                }
            }

            EntityManager.DestroyEntity(query);
        }
    }
}