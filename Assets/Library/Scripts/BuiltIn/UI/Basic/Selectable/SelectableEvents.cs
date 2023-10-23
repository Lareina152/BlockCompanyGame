using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class SelectableEvents : MonoBehaviour
{
    [LabelText("����갴��")]
    public UnityEvent onPointerDown = new();

    [LabelText("�����̧��")]
    public UnityEvent onPointerUp = new();

    [LabelText("��������")]
    public UnityEvent onPointerEnter = new();

    [LabelText("������뿪")]
    public UnityEvent onPointerExit = new();
}
