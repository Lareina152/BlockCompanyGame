using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Newtonsoft.Json;

public class DebugUIPanelPreset : UIToolkitPanelPreset
{
    public const string registeredID = "debug_screen_ui";

    public const string DEBUGGING_UI_SETTING_CATEGORY = "调试面板设置";

    public override bool autoAddToPrefabList => true;

    public override Type controllerType => typeof(DebugUIPanelController);

    [LabelText("左容器VisualElement名称"), FoldoutGroup(DEBUGGING_UI_SETTING_CATEGORY, Expanded = false)]
    [ValueDropdown(nameof(GetVisualTreeNames))]
    [StringIsNotNullOrEmpty]
    [JsonProperty]
    public string leftContainerVisualElementName;

    [LabelText("右容器VisualElement名称"), FoldoutGroup(DEBUGGING_UI_SETTING_CATEGORY)]
    [ValueDropdown(nameof(GetVisualTreeNames))]
    [StringIsNotNullOrEmpty]
    [JsonProperty]
    public string rightContainerVisualElementName;

    [LabelText("Debug条目的UI模板"), SuffixLabel("UXML文件"), FoldoutGroup(DEBUGGING_UI_SETTING_CATEGORY)]
    [Required]
    public VisualTreeAsset debugEntryVisualTree;

    [LabelText("Debug条目VisualElement名称"), FoldoutGroup(DEBUGGING_UI_SETTING_CATEGORY)]
    [ValueDropdown(nameof(GetDebugEntryVisualTreeNames))]
    [StringIsNotNullOrEmpty]
    [JsonProperty]
    public string debugEntryVisualElementName;

    [LabelText("条目标题VisualElement名称"), FoldoutGroup(DEBUGGING_UI_SETTING_CATEGORY)]
    [ValueDropdown(nameof(GetDebugEntryVisualTreeNamesOfLabel))]
    [StringIsNotNullOrEmpty]
    [JsonProperty]
    public string entryTitleVisualElementName;

    [LabelText("条目内容VisualElement名称"), FoldoutGroup(DEBUGGING_UI_SETTING_CATEGORY)]
    [ValueDropdown(nameof(GetDebugEntryVisualTreeNamesOfLabel))]
    [StringIsNotNullOrEmpty]
    [JsonProperty]
    public string entryContentVisualElementName;

    private IEnumerable<string> GetDebugEntryVisualTreeNames()
    {
        return debugEntryVisualTree.GetAllNames();
    }

    private IEnumerable<string> GetDebugEntryVisualTreeNamesOfLabel()
    {
        return debugEntryVisualTree.GetAllNames<Label>();
    }
}
