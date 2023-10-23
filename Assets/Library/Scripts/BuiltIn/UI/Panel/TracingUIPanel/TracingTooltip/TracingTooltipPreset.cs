using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Newtonsoft.Json;
using UnityEngine;

public class TracingTooltipPreset : TracingUIPanelPreset
{
    public const string TOOLTIP_SETTING_CATEGORY = "提示框设置";

    public override Type controllerType => typeof(TracingTooltipController);

    [LabelText("提示框优先级"), FoldoutGroup(TOOLTIP_SETTING_CATEGORY)]
    [JsonProperty]
    public int tooltipPriority;

    [LabelText("标题Label名称"), FoldoutGroup(TOOLTIP_SETTING_CATEGORY)]
    [ValueDropdown(nameof(GetVisualTreeNamesOfLabel))]
    [StringIsNotNullOrEmpty]
    [JsonProperty]
    public string titleLabelName;

    [LabelText("描述Label名称"), FoldoutGroup(TOOLTIP_SETTING_CATEGORY)]
    [ValueDropdown(nameof(GetVisualTreeNamesOfLabel))]
    [StringIsNotNullOrEmpty]
    [JsonProperty]
    public string descriptionLabelName;

    [LabelText("属性容器名称"), FoldoutGroup(TOOLTIP_SETTING_CATEGORY)]
    [ValueDropdown(nameof(GetVisualTreeNames))]
    [StringIsNotNullOrEmpty]
    [JsonProperty]
    public string attributeContainerName;

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        isUnique = true;
    }

    public override void CheckSettings()
    {
        base.CheckSettings();

        isUnique = true;
    }
}
