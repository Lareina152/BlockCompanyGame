#if UNITY_EDITOR

using System;
using Basis;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ToDoList
{
    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    [Serializable]
    public class Info
    {
        [MultiLineProperty]
        [HideLabel]
        public string content;
    }

    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    [Serializable]
    public class ActiveBugInfo : Info
    {
        [LabelText("是否修复")]
        [OnValueChanged(nameof(OnIsFixedChanged))]
        public bool isFixed = false;

        public void OnIsFixedChanged()
        {
            if (isFixed)
            {
                if (GameCoreSettingBase.toDoListTools.activeBugs.Remove(item => item.content == content))
                {
                    GameCoreSettingBase.toDoListTools.fixedBugs.Add(this);
                }
            }
        }
    }

    [CreateAssetMenu(fileName = "ToDoListTools", menuName = "ToolsConfiguration/ToDoListTools")]
    public class ToDoListTools : GeneralSettingBase
    {
        public override StringTranslation settingName => new()
        {
            { "Chinese", "待办清单" },
            { "English", "To-Do List" }
        };

        public override bool ignoreGeneralSettingsInGameEditor => true;

        [LabelText("待办清单")]
        [GUIColor(0.3f, 0.8f, 0.8f)]
        public List<Info> toDoList = new();

        [LabelText("待修复BUGS")]
        [GUIColor(1, 0.5f, 0.5f)]
        public List<ActiveBugInfo> activeBugs = new();

        [LabelText("已修复BUGS")]
        [GUIColor(0.5f, 1, 0.5f)]
        public List<Info> fixedBugs = new();
    }
}

#endif
