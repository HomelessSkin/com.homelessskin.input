using Unity.Entities;

namespace Input
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    [UpdateAfter(typeof(CollectSystem))]
    public partial class UISystem : HandleSystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            Group = Controller.GroupTag.UI;
        }
    }
}