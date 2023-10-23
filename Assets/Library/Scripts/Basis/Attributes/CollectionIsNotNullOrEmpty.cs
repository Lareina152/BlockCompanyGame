using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#if UNITY_EDITOR

using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

#endif

[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class CollectionIsNotNullOrEmpty : Attribute
{
    
}

#if UNITY_EDITOR

public class CollectionIsNotNullOrEmptyDrawer : OdinAttributeDrawer<CollectionIsNotNullOrEmpty>
{
    public override bool CanDrawTypeFilter(Type type)
    {
        return typeof(ICollection).IsAssignableFrom(type);
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
        var value = Property.ValueEntry.WeakSmartValue;

        if (value is ICollection { Count: 0 })
        {
            SirenixEditorGUI.ErrorMessageBox("不能为空");
        }

        CallNextDrawer(label);
    }
}

#endif


