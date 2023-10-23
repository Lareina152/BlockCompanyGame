using System.Collections;
using System.Collections.Generic;
using Basis;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

public class TracingUIPanelPreset : UIToolkitPanelPreset
{
    protected const string TRACING_UI_SETTING_CATEGORY = "鼠标追随UI的设置";

    [LabelText("默认中心点"), FoldoutGroup(TRACING_UI_SETTING_CATEGORY, Expanded = false)]
    [SuffixLabel("左下角为(0, 0)")]
    [MinValue(0), MaxValue(1)]
    [JsonProperty]
    public Vector2 defaultPivot = new(0, 1);

    [LabelText("允许溢出屏幕"), FoldoutGroup(TRACING_UI_SETTING_CATEGORY)]
    [JsonProperty]
    public bool overflowEnable = false;

    [LabelText("自动纠正中心点"), FoldoutGroup(TRACING_UI_SETTING_CATEGORY)]
    [HideIf(nameof(overflowEnable))]
    [JsonProperty]
    public bool autoPivotCorrection = true;

    [LabelText("鼠标跟随"), FoldoutGroup(TRACING_UI_SETTING_CATEGORY)]
    [JsonProperty]
    [ToggleButtons("持续跟随", "仅跟随一次")]
    public bool continuousMouseTracing = true;

    [LabelText("使用绝对位置的Right进行左右定位"), FoldoutGroup(TRACING_UI_SETTING_CATEGORY)]
    [LabelWidth(200)]
    [JsonProperty]
    public bool useRightPosition = false;

    [LabelText("使用绝对位置的Top进行上下定位"), FoldoutGroup(TRACING_UI_SETTING_CATEGORY)]
    [LabelWidth(200)]
    [JsonProperty]
    public bool useTopPosition = false;

    [LabelText("容器VisualElement名称"), FoldoutGroup(TRACING_UI_SETTING_CATEGORY)]
    [ValueDropdown(nameof(GetVisualTreeNames))]
    [StringIsNotNullOrEmpty]
    public string containerVisualElementName;

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
