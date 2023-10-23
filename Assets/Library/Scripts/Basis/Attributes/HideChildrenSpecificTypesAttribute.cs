using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

using UnityEngine;


[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
public sealed class HideChildrenSpecificTypesAttribute : Attribute
{
    public Type[] types;

    public HideChildrenSpecificTypesAttribute(params Type[] types)
    {
        this.types = types;
    }
}

#if UNITY_EDITOR

public sealed class HideSpecificTypesAttributeDrawer : OdinAttributeDrawer<HideChildrenSpecificTypesAttribute>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        foreach (var child in Property.Children)
        {
            if (child.ValueEntry == null)
            {
                continue;
            }

            if (Attribute.types.Contains(child.ValueEntry.TypeOfValue))
            {
                child.State.Visible = false;
            }
        }

        CallNextDrawer(label);
    }
}

#endif