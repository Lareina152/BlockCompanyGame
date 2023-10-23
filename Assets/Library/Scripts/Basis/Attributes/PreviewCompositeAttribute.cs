using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Basis;
using UnityEngine;
using Sirenix.OdinInspector;
using Debug = UnityEngine.Debug;


#if UNITY_EDITOR

using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;

#endif

[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
public class PreviewCompositeAttribute : Attribute
{
    public string previewContent;

    public PreviewCompositeAttribute(string previewContent = "")
    {
        this.previewContent = previewContent;
    }
}

#if UNITY_EDITOR
public class PreviewCompositeAttributeDrawer : OdinAttributeDrawer<PreviewCompositeAttribute>
{
    private ValueResolver<string> previewContent;

    protected override void Initialize()
    {
        previewContent = ValueResolver.GetForString(Property, Attribute.previewContent);
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
        CallNextDrawer(label);

        if (label != null)
        {
            var position = Property.LastDrawnValueRect;

            position = position.TakeFromTop(EditorGUIUtility.singleLineHeight).
                HorizontalPadding(GUIHelper.BetterLabelWidth + 4f, 8f);

            GUI.Label(position, previewContent.GetValue());
        }
    }
}

#endif
