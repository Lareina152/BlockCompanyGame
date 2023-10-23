using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;

using Basis;

[AddComponentMenu("CustomEvents/WindowEvent", 0)]
public class WindowEvent : MonoBehaviour
{
    [ShowInInspector]
    [ReadOnly]
    [BoxGroup("静态变量")]
    public static WindowEvent focusedWindow = default(WindowEvent);

    // [ShowInInspector]
    // [ReadOnly]
    // [BoxGroup("静态变量")]
    // public static WindowEvent mouseInWindow = default(WindowEvent);


    [LabelText("窗口事件名称")]
    public string windowEventName = "";

    [LabelText("是否自动获取焦点")]
    public bool autoFocused = false;

    [LabelText("是否为开启状态")]
    public bool isOn = true;

    [LabelText("是否为Debug模式")]
    public bool isDebugging = false;

    bool isFocused = false;

    [FoldoutGroup("窗口焦点")]
    [Header("获得窗口焦点时执行")]
    public UnityEvent<WindowEvent> focusOn;
    [FoldoutGroup("窗口焦点")]
    [Header("失去窗口焦点时执行")]
    public UnityEvent<WindowEvent> focusOff;
    [FoldoutGroup("窗口焦点")]
    [Header("获得或失去窗口焦点时执行")]
    public UnityEvent<WindowEvent> focusToggle;

    [FoldoutGroup("窗口开关")]
    [Header("窗口打开时执行")]
    public UnityEvent<WindowEvent> windowOn;
    [FoldoutGroup("窗口开关")]
    [Header("窗口关闭时执行")]
    public UnityEvent<WindowEvent> windowOff;
    [FoldoutGroup("窗口开关")]
    [Header("窗口打开或关闭时执行")]
    public UnityEvent<WindowEvent> windowToggle;

    // List<WindowEvent> windowParents = new List<WindowEvent>();
    // List<WindowEvent> windowChildren = new List<WindowEvent>();

    void Awake() {
        // windowParents = GameObjectFunc.FindParentsComponents<WindowEvent>(this);
        // windowChildren = GameObjectFunc.FindChildrenComponents<WindowEvent>(this);
    }

    void Start() {
        if (autoFocused == true) {
            Focus();
        }
    }

    public void TurnWindowOn() {
        if (isOn == false) {
            windowOn.Invoke(this);
            windowToggle.Invoke(this);
            isOn = true;
        }

        if (isDebugging == true) {
            _DisplayCurrentState();
        }
    }

    public void TurnWindowOff() {
        if (isOn == true) {
            windowOff.Invoke(this);
            windowToggle.Invoke(this);
            isOn = false;
        }

        if (isDebugging == true) {
            _DisplayCurrentState();
        }
    }

    public void ToggleWindowOnOff() {
        if (isOn == true) {
            TurnWindowOff();
        } else {
            TurnWindowOn();
        }
    }

    [Button(ButtonSizes.Medium, Icon = SdfIconType.ExclamationLg)]
    public void Focus(bool focusParent = true) {
        if (isFocused == false) {
            focusOn.Invoke(this);
            focusToggle.Invoke(this);
            isFocused = true;
        }
        if (focusedWindow != this) {
            if (focusedWindow != default(WindowEvent)) {
                focusedWindow.Defocus();
            }
            focusedWindow = this;
        }

        if (isDebugging == true) {
            _DisplayCurrentState();
        }
    }

    public void Defocus() {
        if (isFocused == true) {
            focusOff.Invoke(this);
            focusToggle.Invoke(this);
            isFocused = false;
        }
        if (focusedWindow == this) {
            focusedWindow = default(WindowEvent);
        }

        if (isDebugging == true) {
            _DisplayCurrentState();
        }
    }

    public void ToggleFocus() {
        if (isFocused == true) {
            Defocus();
        } else {
            Focus();
        }
    }

    //Only for Debugging
    public void _DisplayCurrentState() {
        Note.note.Log($"isFocused: {isFocused}, isOn: {isOn}");
    }

    //Only for Debugging
    public void _DisplayThisObject() {
        Note.note.Log($"name:{gameObject.name}, tag:{gameObject.tag}, position: {transform.position}");
    }
}
