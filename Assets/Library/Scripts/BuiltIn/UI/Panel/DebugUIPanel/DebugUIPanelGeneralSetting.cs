using Basis;
using System.Collections;
using System.Collections.Generic;
using Basis.GameItem;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

public enum DebugEntryPosition
{
    [LabelText("左")]
    Left,
    [LabelText("右")]
    Right
}

public class DebugEntry : GamePrefabCoreBundle<DebugEntry, DebugUIPanelGeneralSetting>.GameItemPrefab
{
    [LabelText("位置")]
    [EnumToggleButtons]
    [JsonProperty]
    public DebugEntryPosition position;

    [LabelText("标题格式")]
    [JsonProperty]
    public LabelGUIFormat titleFormat = new();

    [LabelText("内容格式")]
    [JsonProperty]
    public LabelGUIFormat contentFormat = new();

    public virtual string OnHandleContentUpdate() => "";

    public virtual bool ShouldDisplay() => true;

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        titleFormat ??= new();
        contentFormat ??= new();
    }
}

public class DebugUIPanelGeneralSetting : GamePrefabCoreBundle<DebugEntry, DebugUIPanelGeneralSetting>.GameItemGeneralSetting
{
    public override StringTranslation settingName => new()
    {
        { "Chinese", "Debug UI面板" },
        { "English", "Debug UI Panel" }
    };

    public override string fullSettingName => settingName;

    public override bool hasIcon => true;

    public override SdfIconType icon => SdfIconType.Bug;

    public override StringTranslation prefabName => new()
    {
        { "Chinese", "Debug条目" },
        { "English", "Debug Entry" }
    };

    public override StringTranslation prefabSuffixName => "";

    public override StringTranslation folderPath => GameCoreSettingBase.uiPanelGeneralSetting.fullSettingName;

    public override bool gameTypeAbandoned => true;

    [LabelText("更新间隔"), SuffixLabel("秒")]
    [JsonProperty]
    [PropertyRange(0.1f, 1f)]
    public float updateInterval = 0.2f;
}
