using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteAlways]
[RequireComponent(typeof(UINode))]
public class UITree : MonoBehaviour
{
    public void CheckUINode()
    {
        var v = new VisualElement();
        //v.style.width = new StyleLength()
        foreach (var child in transform.GetAllChildren(true))
        {
            var uiNode = child.GetOrAddComponent<UINode>();
        }
    }
}
