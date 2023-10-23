using System;
using System.Linq;
using Basis;
using System.Collections;
using System.Collections.Generic;
using Basis.GameItem;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public sealed class UIPanelGeneralSetting : 
    GamePrefabCoreBundle<UIPanelPreset, UIPanelGeneralSetting>.GameItemGeneralSetting, 
    IManagerCreationProvider
{
    public const string TOOLTIP_SETTING_CATEGORY = "提示框设置";

    public const string PANEL_SETTING_CATEGORY = "面板设置";

    [Serializable]
    public class LanguageConfig : IDBasedConfig
    {
        [LabelText("语言")]
        [ValueDropdown("@GameCoreSettingBase.translationGeneralSetting.GetPrefabNameList()")]
        [StringIsNotNullOrEmpty]
        public string languageID;

        [LabelText("样式表")]
        [Required]
        public StyleSheet styleSheet;

        public override void CheckSettings()
        {
            base.CheckSettings();

            Note.note.AssertIsNotNull(styleSheet, nameof(styleSheet));
        }

        public override string GetID() => languageID;
    }

    public override StringTranslation settingName => new()
    {
        { "Chinese", "UI面板" },
        { "English", "UI Panel" }
    };

    public override string fullSettingName => settingName;

    public override bool hasIcon => true;

    public override SdfIconType icon => SdfIconType.LayoutWtf;

    public override StringTranslation prefabName => new()
    {
        { "Chinese", "UI面板" },
        { "English", "UI Panel" }
    };

    public override StringTranslation prefabSuffixName => new()
    {
        { "Chinese", "预设" },
        { "English", "Preset" }
    };

    public override bool gameTypeAbandoned => true;

    [HideLabel, TitleGroup("UI面板的容器设置")]
    public ContainerChooser container;

    [LabelText("默认主题"), TitleGroup(PANEL_SETTING_CATEGORY)]
    [Required]
    public ThemeStyleSheet defaultTheme;

    [LabelText("默认屏幕匹配模式"), TitleGroup(PANEL_SETTING_CATEGORY)]
    public PanelScreenMatchMode defaultScreenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;

    [LabelText("默认宽高匹配度"), TitleGroup(PANEL_SETTING_CATEGORY)]
    [PropertyRange(0, 1)]
    public float defaultMatch = 0.5f;

    [LabelText("默认参考分辨率"), TitleGroup(PANEL_SETTING_CATEGORY)]
    public Vector2Int defaultReferenceResolution = new(1920, 1080);

    [LabelText("面板设置字典")]
    [ShowInInspector]
    private Dictionary<int, PanelSettings> panelSettingsBySortingOrder = new();

    [LabelText("语言设置"), TitleGroup("语言设置")]
    public IDBasedConfigContainer<LanguageConfig> languageConfigs = new();

    [LabelText("通用Tooltip"), TitleGroup(TOOLTIP_SETTING_CATEGORY)]
    [ValueDropdown("@GetPrefabNameList(typeof(TracingTooltipPreset))")]
    [StringIsNotNullOrEmpty]
    [JsonProperty]
    public string universalTooltipID;

    #region GUI

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        container ??= new();

        var testContainer = container.GetContainer();

        Note.note.AssertIsNotNull(testContainer, nameof(testContainer));
    }

    #endregion

    protected override void OnPreInit()
    {
        base.OnPreInit();

        panelSettingsBySortingOrder.Clear();

        foreach (var prefab in GetAllPrefabs())
        {
            if (panelSettingsBySortingOrder.ContainsKey(prefab.sortingOrder) == false)
            {
                panelSettingsBySortingOrder[prefab.sortingOrder] = CreateInstance<PanelSettings>();
            }
        }

        foreach (var (sortingOrder, panelSetting) in panelSettingsBySortingOrder)
        {
            panelSetting.sortingOrder = sortingOrder;
            panelSetting.themeStyleSheet = defaultTheme;
            panelSetting.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            panelSetting.screenMatchMode = defaultScreenMatchMode;
            panelSetting.match = defaultMatch;
            panelSetting.referenceResolution = defaultReferenceResolution;
        }
    }

    public override void CheckSettings()
    {
        base.CheckSettings();

        languageConfigs.CheckSettings();
    }

    private IEnumerable<PanelSettings> GetAllPanelSettings()
    {
        return panelSettingsBySortingOrder.Values;
    }

    public PanelSettings GetPanelSetting(int sortingOrder)
    {
        return panelSettingsBySortingOrder[sortingOrder];
    }

    public IEnumerable<ManagerType> GetManagerTypes()
    {
        yield return ManagerType.UICore;
    }

    public void HandleManagerCreation(ManagerType managerType, Transform managerContainer)
    {
        managerContainer.GetOrAddComponent<UIPanelManager>();
    }
}