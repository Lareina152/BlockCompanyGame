using Basis;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Basis.GameItem;
using UnityEngine;

public class RegisteredTrail : 
    GamePrefabCoreBundle<RegisteredTrail, TrailSpawnerGeneralSetting>.GameItemPrefab
{
    [LabelText("��βԤ����")]
    [AssetList]
    [AssetSelector(Paths = "Assets")]
    [AssetsOnly]
    [Required]
    public TrailRenderer trailPrefab;
}

public class TrailSpawnerGeneralSetting : 
    GamePrefabCoreBundle<RegisteredTrail, TrailSpawnerGeneralSetting>.GameItemGeneralSetting
{
    public override StringTranslation settingName => new()
    {
        { "Chinese", "拖尾生成器" },
        { "English", "Trail Spawner" }
    };

    public override StringTranslation folderPath => visualEffectCategory;

    public override StringTranslation prefabName => new()
    {
        { "Chinese", "注册的拖尾" },
        { "English", "Registered Trail" }
    };

    public override StringTranslation prefabSuffixName => "";

    protected override bool showSetFirstPrefabIDAndNameToDefaultButton => false;

    public override bool gameTypeAbandoned => true;

    [HideLabel]
    public ContainerChooser container = new();

    #region GUI

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        container ??= new();
        container.SetDefaultContainerID("TrailContainer");
    }

    #endregion
}
