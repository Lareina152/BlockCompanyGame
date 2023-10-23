using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class SelectableEvents : MonoBehaviour
{
    [LabelText("当鼠标按下")]
    public UnityEvent onPointerDown = new();

    [LabelText("当鼠标抬起")]
    public UnityEvent onPointerUp = new();

    [LabelText("当鼠标进入")]
    public UnityEvent onPointerEnter = new();

    [LabelText("当鼠标离开")]
    public UnityEvent onPointerExit = new();
}
