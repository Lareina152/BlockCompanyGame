using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Basis;
using UnityEngine;

#if UNITY_EDITOR

using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

#endif

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
[Conditional("UNITY_EDITOR")]
public class DisallowDuplicateElementsAttribute : Attribute
{
    
}

#if UNITY_EDITOR

public class DisallowDuplicateElementsDrawer<T> : OdinAttributeDrawer<DisallowDuplicateElementsAttribute, T>
where T : IEnumerable
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        var value = ValueEntry.SmartValue;

        if (value.Cast<object>().ContainsSame())
        {
            SirenixEditorGUI.ErrorMessageBox("不能包含重复元素");
        }

        CallNextDrawer(label);
    }
}

#endif
