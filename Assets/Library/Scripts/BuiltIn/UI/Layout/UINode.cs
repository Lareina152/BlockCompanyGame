using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteAlways]
public class UINode : MonoBehaviour
{
    [ShowInInspector, ReadOnly]
    private UITree uiTree;

    private void OnTransformChildrenChanged()
    {
        if (uiTree == null)
        {
            FindUITree();
        }

        if (uiTree != null)
        {
            uiTree.CheckUINode();
        }
    }

    private void Reset()
    {
        FindUITree();
    }

    private void FindUITree()
    {
        uiTree = transform.QueryComponentOnParents<UITree>();
    }
}
