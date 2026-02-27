using Core;

using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public abstract partial class PointerSystem : UnmanagedSingletonSystem<PhysicsWorldSingleton>
    {
        public static void Init(PointerSettings settings) => Settings = settings;
        protected static PointerSettings Settings;

        protected MouseState Now;
        protected MouseState Prev;

        protected Vector2 MFrom;
        protected Vector2 MCurrent;

        protected Vector2 MDelta { get { return MCurrent - MFrom; } }

        float T;

        /// <summary>
        /// Conflicted SetState Method Calls
        /// </summary>
        protected override void OnUpdate()
        {
            var query = SystemAPI.QueryBuilder().WithAll<SetPointerStateRequest>().Build();
            if (!query.IsEmpty)
            {
                var requests = query.ToComponentDataArray<SetPointerStateRequest>(Allocator.Temp);
                for (int r = 0; r < requests.Length; r++)
                    SetState(requests[r].State);

                EntityManager.DestroyEntity(query);
            }

            base.OnUpdate();
        }
        protected override void Proceed()
        {
            if (!Settings)
                return;

            T += SystemAPI.Time.DeltaTime;
            if (T < Settings.Freequency)
                return;
            T = 0f;

            MFrom = MCurrent;
            var mouse = Mouse.current;
            MCurrent = mouse.position.ReadValue();

            switch (Now)
            {
                case MouseState.UI:
                {
                    UIAction();

                    SetState(MouseState.Up);
                }
                break;
                case MouseState.Up:
                {
                    if (mouse.leftButton.isPressed)
                    {
                        ClickAction();

                        SetState(MouseState.Down);
                    }
                    else if (MDelta.magnitude > 0.001f)
                        UpSlideAction();
                    else
                        UpAction();
                }
                break;
                case MouseState.Down:
                {
                    if (!mouse.leftButton.isPressed)
                    {
                        ReleaseAction();

                        SetState(MouseState.Up);
                    }
                    else if (MDelta.magnitude > 0.001f)
                        SetState(MouseState.Slide);
                }
                break;
                case MouseState.Slide:
                {
                    if (!mouse.leftButton.isPressed)
                    {
                        ReleaseAction();

                        SetState(MouseState.Up);
                    }
                    else if (MDelta.magnitude <= 0.001f)
                        SetState(MouseState.Down);
                    else
                        DownSlideAction();
                }
                break;
            }
        }

        protected virtual void SetState(MouseState state)
        {
            Prev = Now;
            Now = state;
        }
        protected virtual void UIAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "UI");
        }
        protected virtual void UpAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "Up");
        }
        protected virtual void ClickAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "Click");
        }
        protected virtual void ReleaseAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "Release");
        }
        protected virtual void UpSlideAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "UpSlide");
        }
        protected virtual void DownSlideAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "DownSlide");
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