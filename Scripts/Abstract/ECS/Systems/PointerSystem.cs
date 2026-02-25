using Core;

using Unity.Collections;
using Unity.Entities;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public abstract partial class PointerSystem : BehaviourSystem
    {
        protected MouseState Now;
        protected MouseState Prev;

        protected Vector2 MFrom;
        protected Vector2 MCurrent;

        protected Vector2 MDelta { get { return MCurrent - MFrom; } }

        protected Camera PlayerCamera;

        protected override void GetRef()
        {
            if (!PlayerCamera)
            {
                var go = GameObject.FindGameObjectWithTag("MainCamera");
                if (go)
                    PlayerCamera = go.GetComponent<Camera>();
            }
        }
        protected override void OnUpdate()
        {
            var query = SystemAPI.QueryBuilder().WithAll<SetPointerStateRequest>().Build();
            {
                var requests = query.ToComponentDataArray<SetPointerStateRequest>(Allocator.Temp);
                for (int r = 0; r < requests.Length; r++)
                    SetState(requests[r].State);
            }
            EntityManager.DestroyEntity(query);

            base.OnUpdate();
        }
        protected override void Proceed()
        {
            MFrom = MCurrent;
            var mouse = Mouse.current;
            MCurrent = mouse.position.ReadValue();

            switch (Now)
            {
                case MouseState.UI:
                {
                    SetState(MouseState.Up);
                }
                break;
                case MouseState.Up:
                {
                    if (mouse.leftButton.isPressed)
                        SetState(MouseState.Down);
                    else
                        PerformUpAction();
                }
                break;
                case MouseState.Down:
                {
                    if (!mouse.leftButton.isPressed)
                        SetState(MouseState.Up);
                    else if ((mouse.position.ReadValue() - MFrom).magnitude > 0.001f)
                        SetState(MouseState.Slide);
                    else
                        PerformClickAction();
                }
                break;
                case MouseState.Slide:
                {
                    if (!mouse.leftButton.isPressed)
                        SetState(MouseState.Up);
                    else if ((MCurrent - MFrom).magnitude <= 0.001f)
                        SetState(MouseState.Down);
                    else
                        PerformSlideAction();
                }
                break;
            }
        }

        protected virtual void SetState(MouseState state)
        {
            switch (state)
            {
                case MouseState.Down:
                break;
            }

            Prev = Now;
            Now = state;
        }
        protected virtual void PerformClickAction()
        {
        }
        protected virtual void PerformSlideAction()
        {
        }
        protected virtual void PerformUpAction()
        {
        }
    }

    public enum MouseState : byte
    {
        Up = 0,
        UI = 1,
        Down = 2,
        Slide = 3,
    }

    public struct SetPointerStateRequest : IComponentData
    {
        public MouseState State;
    }
}