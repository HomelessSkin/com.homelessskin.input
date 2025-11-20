using Core.Util;

using UI;

using Unity.Entities;

namespace Input
{
    public abstract partial class InputHandleSystem : ReloadSingletoneSystem<Input>
    {
        protected override void Proceed()
        {

        }
    }

    [UpdateInGroup(typeof(InputSystemGroup))]
    [UpdateAfter(typeof(InputCollectSystem))]
    public partial class UIInputSystem : InputHandleSystem
    {
        UIManagerBase UIManager;

        protected override void OnCreate()
        {
            base.OnCreate();

            RequireForUpdate<Input>();
        }

        protected override void Proceed()
        {
            base.Proceed();


        }
    }
}