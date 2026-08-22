using Core;

using Unity.Collections;
using Unity.Entities;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public abstract partial class PointerSystem : BehaviourSystem
    {
        public static Vector2 Delta { get; private set; }

        protected MouseState Now;
        protected MouseState Prev;

        protected Vector2 MScroll;

        protected Controller Controller;
        protected PointerSettings Settings;

        bool LeftIsPressed;
        bool RightIsPressed;
        float T;

        protected override void GetRef()
        {
            if (!Controller)
            {
                var controllers = GameObject.FindGameObjectsWithTag("InputController");
                if (controllers != null)
                    for (int c = 0; c < controllers.Length; c++)
                    {
                        var controller = controllers[c].GetComponent<Controller>();
                        if (controller?.GetGroup() == Group)
                        {
                            Controller = controller;

                            break;
                        }
                    }
            }

            if (!Settings &&
                   Controller)
                Settings = Controller.GetPointerSettings();
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
            if (!Settings)
                return;

            if (Settings.LogActivity)
                Log.Info(this, "Is Active");

            var mouse = Mouse.current;

            if (mouse.leftButton.wasPressedThisFrame)
                LeftIsPressed = true;
            if (mouse.leftButton.wasReleasedThisFrame)
                LeftIsPressed = false;
            if (mouse.rightButton.wasPressedThisFrame)
                RightIsPressed = true;
            if (mouse.rightButton.wasReleasedThisFrame)
                RightIsPressed = false;

            MScroll = mouse.scroll.ReadValue();
            if (MScroll.y > 0f)
                UpScrollAction();
            else if (MScroll.y < 0f)
                DownScrollAction();

            T -= SystemAPI.Time.DeltaTime;
            if (T > 0f)
                return;
            T += Settings.Freequency;

            Delta = mouse.delta.ReadValue();

            switch (Now)
            {
                case MouseState.UI:
                {
                    UIAction();

                    LeftIsPressed = false;
                    RightIsPressed = false;

                    SetState(MouseState.Up);
                }
                break;
                case MouseState.Up:
                {
                    if (LeftIsPressed)
                    {
                        LeftClickAction();

                        SetState(MouseState.LeftDown);
                    }
                    else if (RightIsPressed)
                    {
                        RightClickAction();

                        SetState(MouseState.RightDown);
                    }
                    else if (Delta.magnitude > 0.001f)
                        UpSlideAction();
                    else
                        UpAction();
                }
                break;
                case MouseState.LeftDown:
                {
                    if (!LeftIsPressed)
                    {
                        ReleaseLeftAction();

                        SetState(MouseState.Up);

                        break;
                    }
                    else if (Delta.magnitude > 0.001f)
                        SetState(MouseState.LeftSlide);

                    LeftHoldAction();
                }
                break;
                case MouseState.RightDown:
                {
                    if (!RightIsPressed)
                    {
                        ReleaseRightAction();

                        SetState(MouseState.Up);

                        break;
                    }
                    else if (Delta.magnitude > 0.001f)
                        SetState(MouseState.RightSlide);

                    RightHoldAction();
                }
                break;
                case MouseState.LeftSlide:
                {
                    if (!LeftIsPressed)
                    {
                        ReleaseLeftAction();

                        SetState(MouseState.Up);

                        break;
                    }
                    else if (Delta.magnitude <= 0.001f)
                        SetState(MouseState.LeftDown);

                    LeftDownSlideAction();
                }
                break;
                case MouseState.RightSlide:
                {
                    if (!RightIsPressed)
                    {
                        ReleaseRightAction();

                        SetState(MouseState.Up);

                        break;
                    }
                    else if (Delta.magnitude <= 0.001f)
                        SetState(MouseState.RightDown);

                    RightDownSlideAction();
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
        protected virtual void LeftClickAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "Left Click");
        }
        protected virtual void RightClickAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "Right Click");
        }
        protected virtual void LeftHoldAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "Hold Left");
        }
        protected virtual void RightHoldAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "Hold Right");
        }
        protected virtual void ReleaseLeftAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "Release Left");
        }
        protected virtual void ReleaseRightAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "Release Right");
        }
        protected virtual void UpSlideAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "Up Slide");
        }
        protected virtual void LeftDownSlideAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "Left Down Slide");
        }
        protected virtual void RightDownSlideAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "Right Down Slide");
        }
        protected virtual void UpScrollAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "Up Scroll");
        }
        protected virtual void DownScrollAction()
        {
            if (Settings.LogActions)
                Log.Info(this, "Down Scroll");
        }
    }

    public enum MouseState : byte
    {
        Up = 0,
        UI = 1,
        LeftDown = 2,
        RightDown = 3,
        LeftSlide = 4,
        RightSlide = 5,
    }

    public struct SetPointerStateRequest : IComponentData
    {
        public MouseState State;
    }
}