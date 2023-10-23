using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ContextMenuUIPreset : TracingUIPanelPreset
{
    public const string registeredID = "context_menu_ui";

    public const string CONTEXT_MENU_UI_CATEGORY = "上下文菜单界面设置";

    public override bool autoAddToPrefabList => true;

    public override Type controllerType => typeof(ContextMenuUIController);

    [LabelText("上下文菜单条目容器名称"), FoldoutGroup(CONTEXT_MENU_UI_CATEGORY)]
    [ValueDropdown(nameof(GetVisualTreeNames))]
    [JsonProperty]
    public string contextMenuEntryContainerName;

    [LabelText("上下文菜单条目资源"), FoldoutGroup(CONTEXT_MENU_UI_CATEGORY)]
    public VisualTreeAsset contextMenuEntryAsset;

    [LabelText("上下文菜单条目标题名称"), FoldoutGroup(CONTEXT_MENU_UI_CATEGORY)]
    [ValueDropdown(nameof(GetVisualTreeNamesOfInputMappingEntry))]
    [JsonProperty]
    public string contextMenuEntryTitleName;

    [LabelText("自动执行上下文菜单条目"), SuffixLabel("如果只有一条"), FoldoutGroup(CONTEXT_MENU_UI_CATEGORY)]
    [JsonProperty]
    public bool autoExecuteIfOnlyOneEntry = true;

    [LabelText("点击的鼠标按键类型"), FoldoutGroup(CONTEXT_MENU_UI_CATEGORY)]
    [JsonProperty]
    public MouseButtonType clickMouseButtonType = MouseButtonType.LeftButton;

    [LabelText("全局事件触发时关闭此UI"), FoldoutGroup(CONTEXT_MENU_UI_CATEGORY)]
    [ValueDropdown("@GameSetting.globalEventSystemGeneralSetting.GetPrefabNameList()")]
    [JsonProperty]
    public List<string> globalEventIDsToClose = new();

    private IEnumerable GetVisualTreeNamesOfInputMappingEntry()
    {
        return contextMenuEntryAsset.GetAllNames();
    }

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        globalEventIDsToClose ??= new();
    }
}
