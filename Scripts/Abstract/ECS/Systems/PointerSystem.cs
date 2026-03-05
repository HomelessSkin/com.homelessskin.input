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
        public static Vector2 Current;
        public static void Init(PointerSettings settings) => Settings = settings;
        protected static PointerSettings Settings;

        protected MouseState Now;
        protected MouseState Prev;

        protected Vector2 MFrom;
        protected Vector2 MCurrent;
        protected Vector2 MScroll;

        protected Camera PlayerCamera;

        protected Vector2 MDelta { get { return MCurrent - MFrom; } }

        bool IsPressed;
        float T;

        protected override void GetRef()
        {
            if (!PlayerCamera)
            {
                var go = GameObject.FindGameObjectWithTag("MainCamera");
                if (go)
                    PlayerCamera = go.GetComponent<Camera>();
            }
        }
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
            if (Settings.LogActivity)
                Log.Info(this, "Is Active");

            var mouse = Mouse.current;

            if (mouse.leftButton.wasPressedThisFrame)
                IsPressed = true;
            if (mouse.leftButton.wasReleasedThisFrame)
                IsPressed = false;

            MScroll = mouse.scroll.ReadValue();
            if (MScroll.y > 0f)
                UpScrollAction();
            else if (MScroll.y < 0f)
                DownScrollAction();

            T += SystemAPI.Time.DeltaTime;
            if (T < Settings.Freequency)
                return;
            T = 0f;

            MFrom = MCurrent;
            Current = MCurrent = mouse.position.ReadValue();

            switch (Now)
            {
                case MouseState.UI:
                {
                    UIAction();

                    IsPressed = false;

                    SetState(MouseState.Up);
                }
                break;
                case MouseState.Up:
                {
                    if (IsPressed)
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
                    if (!IsPressed)
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
                    if (!IsPressed)
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

            if (Settings.LogStates)
                Log.Info(this, $"{state}");
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
        protected virtual void UpScrollAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "UpScroll");
        }
        protected virtual void DownScrollAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "DownScroll");
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