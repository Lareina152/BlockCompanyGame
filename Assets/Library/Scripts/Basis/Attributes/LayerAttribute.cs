using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#if UNITY_EDITOR

using Sirenix.OdinInspector.Editor;
using UnityEditor;

#endif

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
[Conditional("UNITY_EDITOR")]
public class LayerAttribute : Attribute
{

}

#if UNITY_EDITOR

[DrawerPriority(DrawerPriorityLevel.AttributePriority)]
public class LayerAttributeDrawer : OdinAttributeDrawer<LayerAttribute, int>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        this.ValueEntry.SmartValue = label == null ? EditorGUILayout.LayerField(this.ValueEntry.SmartValue) : EditorGUILayout.LayerField(label, this.ValueEntry.SmartValue);
    }
}

#endif