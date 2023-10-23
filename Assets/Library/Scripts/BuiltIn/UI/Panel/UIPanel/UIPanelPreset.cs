using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Basis;
using Basis.GameItem;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class UIPanelPreset : 
    GamePrefabCoreBundle<UIPanelPreset, UIPanelGeneralSetting>.GameItemPrefab
{
    protected override string preferredIDSuffix => "ui";

    [LabelText("控制器类别"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [ShowInInspector]
    public virtual Type controllerType => typeof(UIPanelController);

    [LabelText("显示优先级"), SuffixLabel("大显示优先级的UI会覆盖小的"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [JsonProperty]
    public int sortingOrder = 0;

    [LabelText("唯一"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [JsonProperty]
    public bool isUnique = true;

    [LabelText("创建时自动打开"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [JsonProperty]
    public bool autoOpenOnCreation = false;

    [LabelText("开启此面板时关闭的全局事件"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [ValueDropdown("@GameSetting.globalEventSystemGeneralSetting.GetPrefabNameList()")]
    [ListDrawerSettings(ShowFoldout = false)]
    [DisallowDuplicateElements]
    [JsonProperty]
    public List<string> globalEventDisabledListOnOpen = new();

    [LabelText("鼠标进入时关闭的全局映射"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [ValueDropdown("@GameSetting.globalEventSystemGeneralSetting.GetPrefabNameList()")]
    [ListDrawerSettings(ShowFoldout = false)]
    [DisallowDuplicateElements]
    [JsonProperty]
    public List<string> globalEventDisabledListOnMouseEnter = new();

    [LabelText("启用关闭此UI的输入映射"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [JsonProperty]
    public bool enableCloseInputMapping = false;

    [LabelText("关闭此UI的输入映射"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [ValueDropdown("@GameSetting.globalEventSystemGeneralSetting.GetPrefabNameList()")]
    [StringIsNotNullOrEmpty]
    [ShowIf(nameof(enableCloseInputMapping))]
    [JsonProperty]
    public string closeInputMappingID;

    [LabelText("启用切换开关此UI的输入映射"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [JsonProperty]
    public bool enableToggleInputMapping = false;

    [LabelText("切换开关此UI的输入映射"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [ValueDropdown("@GameSetting.globalEventSystemGeneralSetting.GetPrefabNameList()")]
    [StringIsNotNullOrEmpty]
    [ShowIf(nameof(enableToggleInputMapping))]
    [JsonProperty]
    public string toggleInputMappingID;

    [LabelText("在进入流程时自动打开"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [ValueDropdown("@GameCoreSettingBase.procedureGeneralSetting.GetPrefabNameList()")]
    [DisallowDuplicateElements]
    [JsonProperty]
    public List<string> autoOpenOnEnterProcedures = new();

    [LabelText("在离开流程时自动打开"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [ValueDropdown("@GameCoreSettingBase.procedureGeneralSetting.GetPrefabNameList()")]
    [DisallowDuplicateElements]
    [JsonProperty]
    public List<string> autoOpenOnLeaveProcedures = new();

    [LabelText("在进入流程时自动关闭"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [ValueDropdown("@GameCoreSettingBase.procedureGeneralSetting.GetPrefabNameList()")]
    [DisallowDuplicateElements]
    [JsonProperty]
    public List<string> autoCloseOnEnterProcedures = new();

    [LabelText("在离开流程时自动关闭"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [ValueDropdown("@GameCoreSettingBase.procedureGeneralSetting.GetPrefabNameList()")]
    [DisallowDuplicateElements]
    [JsonProperty]
    public List<string> autoCloseOnLeaveProcedures = new();

    #region GUI

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        globalEventDisabledListOnOpen ??= new();
        globalEventDisabledListOnMouseEnter ??= new();

        autoOpenOnEnterProcedures ??= new();
        autoOpenOnLeaveProcedures ??= new();
        autoCloseOnEnterProcedures ??= new();
        autoCloseOnLeaveProcedures ??= new();
    }

#if UNITY_EDITOR
    [Button("打开控制器脚本"), HorizontalGroup(OPEN_SCRIPT_BUTTON_HORIZONTAL_GROUP)]
    private void OpenControllerTypeScript()
    {
        controllerType.OpenScriptOfType();
    }
#endif

    #endregion

    public override void CheckSettings()
    {
        base.CheckSettings();

        Note.note.AssertIsDerivedFrom(controllerType, typeof(UIPanelController), true, false, true);
    }

    #region JSON Serialization

    public bool ShouldSerializecloseInputMappingID()
    {
        return enableCloseInputMapping;
    }

    public bool ShouldSerializetoggleInputMappingID()
    {
        return enableToggleInputMapping;
    }

    #endregion
}
