using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Basis;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class TooltipBindConfig : BaseConfigClass
{
    [LabelText("目标名称")]
    [JsonProperty]
    public string targetName;

    [LabelText("提示框名称")]
    [JsonProperty]
    public string tooltipName;
}

public class UIToolkitPanelPreset : UIPanelPreset
{
    public const string UI_TOOLKIT_PANEL_CATEGORY = "UI Toolkit面板设置";

    public override Type controllerType => typeof(UIToolkitPanelController);

    public PanelSettings panelSettings
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (useDefaultPanelSettings)
            {
                return GameCoreSettingBase.uiPanelGeneralSetting.GetPanelSetting(sortingOrder);
            }

            return customPanelSettings;
        }
    }

    [LabelText("UI模板"), SuffixLabel("UXML文件"), FoldoutGroup(UI_TOOLKIT_PANEL_CATEGORY, false)]
    [Required]
    public VisualTreeAsset visualTree;

    [LabelText("使用默认的面板设置"), FoldoutGroup(UI_TOOLKIT_PANEL_CATEGORY)]
    [JsonProperty]
    public bool useDefaultPanelSettings = true;

    [LabelText("自定义的面板设置"), FoldoutGroup(UI_TOOLKIT_PANEL_CATEGORY)]
    [HideIf(nameof(useDefaultPanelSettings))]
    [Required]
    public PanelSettings customPanelSettings;

    [LabelText("所有节点忽略鼠标事件"), FoldoutGroup(UI_TOOLKIT_PANEL_CATEGORY)]
    [JsonProperty]
    public bool ignoreMouseEvents = false;

    [LabelText("关闭按钮名称"), FoldoutGroup(UI_TOOLKIT_PANEL_CATEGORY)]
    [ValueDropdown(nameof(GetVisualTreeNamesOfButton))]
    [JsonProperty]
    public string closeUIButtonName;

    [LabelText("绑定Tooltip设置"), FoldoutGroup(UI_TOOLKIT_PANEL_CATEGORY)]
    [JsonProperty]
    [ListDrawerSettings(CustomAddFunction = nameof(AddTooltipBindConfigGUI))]
    public List<TooltipBindConfig> tooltipBindConfigs = new();

    [LabelText("提示框淡出时间"), FoldoutGroup(UI_TOOLKIT_PANEL_CATEGORY)]
    [JsonProperty]
    [PropertyRange(0, 1)]
    public float tooltipFadeDuration = 0.4f;

    #region GUI

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        tooltipBindConfigs ??= new();
    }

    private TooltipBindConfig AddTooltipBindConfigGUI()
    {
        return new TooltipBindConfig();
    }

    #endregion

    public override void CheckSettings()
    {
        base.CheckSettings();

        Note.note.AssertIsNotNull(visualTree, nameof(visualTree));

        if (useDefaultPanelSettings == false)
        {
            Note.note.AssertIsNotNull(customPanelSettings, nameof(customPanelSettings));
        }
    }

    #region Visual Tree Names

    protected IEnumerable<string> GetVisualTreeNames()
    {
        return visualTree.GetAllNames();
    }

    protected IEnumerable<string> GetVisualTreeNamesOfLabel()
    {
        return visualTree.GetAllNames<Label>();
    }

    protected IEnumerable<string> GetVisualTreeNamesOfButton()
    {
        return visualTree.GetAllNames<Button>();
    }

    protected IEnumerable<string> GetVisualTreeNamesOfScrollView()
    {
        return visualTree.GetAllNames<ScrollView>();
    }

    protected IEnumerable<string> GetVisualTreeNamesOfProgressBar()
    {
        return visualTree.GetAllNames<ProgressBar>();
    }

    #endregion
}