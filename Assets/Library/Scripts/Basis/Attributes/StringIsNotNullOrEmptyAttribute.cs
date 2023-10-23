using System;
using System.Diagnostics;
using Basis;
using UnityEngine;

#if UNITY_EDITOR

using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;

#endif

[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.Field)]
public sealed class StringIsNotNullOrEmptyAttribute : Attribute
{
    
}

#if UNITY_EDITOR

[DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
public sealed class StringIsNotNullOrEmptyAttributeDrawer : OdinAttributeDrawer<StringIsNotNullOrEmptyAttribute, string>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        if (ValueEntry.SmartValue.IsNullOrEmptyAfterTrim())
        {
            string propertyName;

            if (label == null)
            {
                propertyName = Property.Name;
            }
            else
            {
                propertyName = label.text;
            }

            SirenixEditorGUI.ErrorMessageBox($"{propertyName}不能为空");
        }

        CallNextDrawer(label);
    }
}

#endif