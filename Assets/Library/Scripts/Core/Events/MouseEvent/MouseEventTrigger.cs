using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Basis;
using EnumsNET;
using UnityEngine.Serialization;

#region Enum Defines

public static class MouseButtonTypeUtilities
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasMouseButton(this MouseButtonType mouseButtonType, int mouseButtonID)
    {
        return mouseButtonID switch
        {
            0 => mouseButtonType.HasFlag(MouseButtonType.LeftButton),
            1 => mouseButtonType.HasFlag(MouseButtonType.RightButton),
            2 => mouseButtonType.HasFlag(MouseButtonType.MiddleButton),
            _ => throw new ArgumentOutOfRangeException(nameof(mouseButtonID), mouseButtonID, null)
        };
    }
}

[Flags]
public enum MouseButtonType {
    [LabelText("鼠标左键")]
    LeftButton = 1 << 0,
    [LabelText("鼠标右键")]
    RightButton = 1 << 1,
    [LabelText("鼠标中键")]
    MiddleButton = 1 << 2,
    [LabelText("鼠标任意键")]
    AnyButton = LeftButton | RightButton | MiddleButton,
}

[Flags]
public enum MouseEventType
{
    [LabelText("无事件")]
    None = 0,
    [LabelText("指针进入")]
    PointerEnter = 1 << 0,
    [LabelText("指针离开")]
    PointerLeave = 1 << 1,
    [LabelText("指针悬停")]
    PointerHover = 1 << 2,
    [LabelText("任意鼠标键按下")]
    AnyMouseButtonDown = 1 << 3,
    [LabelText("任意鼠标键松开")]
    AnyMouseButtonUp = 1 << 4,
    [LabelText("任意鼠标键悬停")]
    AnyMouseButtonStay = 1 << 5,
    [LabelText("鼠标左键按下")]
    LeftMouseButtonDown = 1 << 6,
    [LabelText("鼠标左键松开")]
    LeftMouseButtonUp = 1 << 7,
    [LabelText("鼠标左键点击")]
    LeftMouseButtonClick = 1 << 8,
    [LabelText("鼠标左键悬停")]
    LeftMouseButtonStay = 1 << 9,
    [LabelText("鼠标右键按下")]
    RightMouseButtonDown = 1 << 10,
    [LabelText("鼠标右键松开")]
    RightMouseButtonUp = 1 << 11,
    [LabelText("鼠标右键点击")]
    RightMouseButtonClick = 1 << 12,
    [LabelText("鼠标右键悬停")]
    RightMouseButtonStay = 1 << 13,
    [LabelText("鼠标中键按下")]
    MiddleMouseButtonDown = 1 << 14,
    [LabelText("鼠标中键松开")]
    MiddleMouseButtonUp = 1 << 15,
    [LabelText("鼠标中键点击")]
    MiddleMouseButtonClick = 1 << 16,
    [LabelText("鼠标中键悬停")]
    MiddleMouseButtonStay = 1 << 17,
    [LabelText("拖拽开始")]
    DragBegin = 1 << 18,
    [LabelText("拖拽中")]
    DragStay = 1 << 19,
    [LabelText("拖拽结束")]
    DragEnd = 1 << 20,
}

#endregion

[Serializable]
public struct MouseEventConfig
{
    [HideLabel]
    [GUIColor(nameof(GetMouseEventTypeColorGUI))]
    [EnumToggleButtons]
    public MouseEventType type;

    [LabelText("触发")]
    [InfoBox("至少添加一个事件，否则请移除此触发器", InfoMessageType.Warning,
        "@unityEvent.GetPersistentEventCount() <= 0")]
    public UnityEvent unityEvent;

    [HideInInspector]
    public UnityEvent<MouseEventType> eventWithTypeArg; 

    #region GUI

    private Color GetMouseEventTypeColorGUI()
    {
        switch (type)
        {
            case MouseEventType.PointerEnter:
            case MouseEventType.PointerLeave:
            case MouseEventType.PointerHover:
                return new(1f, 0.7f, 0.7f);
            case MouseEventType.AnyMouseButtonDown:
            case MouseEventType.AnyMouseButtonUp:
            case MouseEventType.AnyMouseButtonStay:
                return new(0.7f, 0.7f, 1);
            case MouseEventType.LeftMouseButtonDown:
            case MouseEventType.LeftMouseButtonUp:
            case MouseEventType.LeftMouseButtonClick:
            case MouseEventType.LeftMouseButtonStay:
                return new(0.5f, 1, 1);
            case MouseEventType.RightMouseButtonDown:
            case MouseEventType.RightMouseButtonUp:
            case MouseEventType.RightMouseButtonClick:
            case MouseEventType.RightMouseButtonStay:
                return new(0.7f, 1, 0.7f);
            case MouseEventType.MiddleMouseButtonDown:
            case MouseEventType.MiddleMouseButtonUp:
            case MouseEventType.MiddleMouseButtonClick:
            case MouseEventType.MiddleMouseButtonStay:
                return new(1, 1, 0.5f);
            case MouseEventType.DragBegin:
            case MouseEventType.DragStay:
            case MouseEventType.DragEnd:
                return new(1, 0.5f, 1);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion
}

[DisallowMultipleComponent]
[AddComponentMenu("EventsCustomed/MouseEventTrigger", 0)]
public class MouseEventTrigger : MonoBehaviour
{
    [LabelText("组名称")]
    public string groupName;

    [LabelText("检测种类")]
    [EnumToggleButtons]
    public ObjectDimensions objectDim = ObjectDimensions.THREE_D;

    [LabelText("绑定的窗口")]
    public WindowEvent bindWindow = default(WindowEvent);

    [LabelText("是否开启绑定模式")]
    [PropertyTooltip("若此Trigger中的任意事件触发，会触发绑定Trigger中的对应事件")]
    public bool isBindingMode = false;

    [LabelText("要绑定的Trigger")]
    [ShowIf(nameof(isBindingMode))]
    [Required]
    public MouseEventTrigger bindTrigger;

    [LabelText("是否触发此触发器")]
    [ShowIf(nameof(isBindingMode))]
    public bool enableTriggerThis = true;

    [LabelText("是否允许拖拽")]
    public bool draggable = false;
    [LabelText("触发拖拽的键"), ShowIf(nameof(draggable))]
    public MouseButtonType dragButton = MouseButtonType.LeftButton;

    [LabelText("事件设置")]
    [SerializeField]
    private List<MouseEventConfig> eventConfigs = new();

    [LabelText("Debugging模式")]
    [FoldoutGroup("Only For Debugging")]
    public bool isDebugging = false;

    [LabelText("事件设置字典")]
    [ShowInInspector]
    [FoldoutGroup("Only For Debugging")]
    [ReadOnly, EnableGUI]
    private Dictionary<MouseEventType, MouseEventConfig> eventConfigsDict;

    private void OnTransformParentChanged() {
        bindWindow = gameObject.FindFirstParentComponent<WindowEvent>(true);
    }

    private void Reset() {
        bindWindow = gameObject.FindFirstParentComponent<WindowEvent>(true);
    }

    private void Awake()
    {
        eventConfigsDict = new();

        foreach (var config in eventConfigs)
        {
            if (config.unityEvent.GetPersistentEventCount() <= 0)
            {
                continue;
            }

            if (config.type == MouseEventType.None)
            {
                continue;
            }

            foreach (var eventTypeFlag in config.type.GetFlags())
            {
                eventConfigsDict[eventTypeFlag] = new()
                {
                    type = eventTypeFlag,
                    unityEvent = config.unityEvent,
                    eventWithTypeArg = config.eventWithTypeArg
                };
            }
        }
    }

    public void Invoke(MouseEventType eventType)
    {
        if (isDebugging)
        {
            Debug.Log($"{name}触发了MouseEvent:{eventType}");
        }

        if (isBindingMode == false || enableTriggerThis)
        {
            if (eventConfigsDict.TryGetValue(eventType, out var config))
            {
                config.unityEvent?.Invoke();
                config.eventWithTypeArg?.Invoke(eventType);
            }
        }

        if (isBindingMode && bindTrigger != null)
        {
            bindTrigger.Invoke(eventType);
        }
    }

    public void AddEvent(MouseEventType eventType, UnityAction action)
    {
        Note.note.AssertIsNot(eventType, MouseEventType.None, nameof(eventType));

        foreach (var eventTypeFlag in eventType.GetFlags())
        {
            if (eventConfigsDict.ContainsKey(eventTypeFlag) == false)
            {
                eventConfigsDict[eventTypeFlag] = new()
                {
                    type = eventTypeFlag,
                    unityEvent = new(),
                    eventWithTypeArg = new()
                };
            }

            var config = eventConfigsDict[eventTypeFlag];

            config.unityEvent.AddListener(action);
        }
        
    }

    public void AddEvent(MouseEventType eventType, UnityAction<MouseEventType> action)
    {
        Note.note.AssertIsNot(eventType, MouseEventType.None, nameof(eventType));

        foreach (var eventTypeFlag in eventType.GetFlags())
        {
            if (eventConfigsDict.ContainsKey(eventTypeFlag) == false)
            {
                eventConfigsDict[eventTypeFlag] = new()
                {
                    type = eventTypeFlag,
                    unityEvent = new(),
                    eventWithTypeArg = new()
                };
            }

            var config = eventConfigsDict[eventTypeFlag];

            config.eventWithTypeArg.AddListener(action);
        }
    }

    public void _DisplayThisObject() {
        Note.note.Log($"name:{gameObject.name}, position: {transform.position}");
    }
}
