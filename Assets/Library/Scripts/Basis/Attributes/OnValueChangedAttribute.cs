#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ActionResolvers;
using Sirenix.Utilities.Editor;
#endif

using System;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
public class OnValueChangedAttributeDrawer<T> : OdinAttributeDrawer<OnValueChangedAttribute, T>
{
    private ActionResolver onChangeAction;
    private bool subscribedToOnUndoRedo;

    protected override void Initialize()
    {
        if (Attribute.InvokeOnUndoRedo)
        {
            Property.Tree.OnUndoRedoPerformed += new Action(OnUndoRedo);
            subscribedToOnUndoRedo = true;
        }

        onChangeAction = ActionResolver.Get(Property, Attribute.Action);
        Action<int> action = TriggerAction;
        ValueEntry.OnValueChanged += action;
        if (Attribute.IncludeChildren || typeof(T).IsValueType)
        {
            ValueEntry.OnChildValueChanged += action;
        }

        //foreach (var child in Property.Children)
        //{
        //    if (child.ValueEntry != null)
        //    {
        //        child.ValueEntry.OnValueChanged += action;
        //    }


        //}

        if (!Attribute.InvokeOnInitialize || onChangeAction.HasError)
        {
            return;
        }

        onChangeAction.DoActionForAllSelectionIndices();
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
        if (onChangeAction.HasError)
        {
            SirenixEditorGUI.ErrorMessageBox(onChangeAction.ErrorMessage);
        }

        CallNextDrawer(label);
    }

    private void OnUndoRedo()
    {
        for (int selectionIndex = 0; selectionIndex < ValueEntry.ValueCount; ++selectionIndex)
            TriggerAction(selectionIndex);
    }

    private void TriggerAction(int selectionIndex) =>
        Property.Tree.DelayActionUntilRepaint(() => onChangeAction.DoAction(selectionIndex));

    public void Dispose()
    {
        if (!subscribedToOnUndoRedo)
            return;
        Property.Tree.OnUndoRedoPerformed -= OnUndoRedo;
    }
}
#endif