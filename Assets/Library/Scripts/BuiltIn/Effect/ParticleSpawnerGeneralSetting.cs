using Basis;
using System.Collections;
using System.Collections.Generic;
using Basis.GameItem;
using ConfigurationBasis;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class ParticleSpawnerGeneralSetting : 
    GamePrefabCoreBundle<RegisteredParticle, ParticleSpawnerGeneralSetting>.GameItemGeneralSetting
{
    public override StringTranslation settingName => new()
    {
        { "Chinese", "粒子生成器" },
        { "English", "Particle Spawner" }
    };

    public override StringTranslation prefabName => new()
    {
        { "Chinese", "注册的粒子系统" },
        { "English", "Registered Particle System" }
    };

    public override StringTranslation folderPath => visualEffectCategory;

    protected override bool showSetFirstPrefabIDAndNameToDefaultButton => false;

    public override bool gameTypeAbandoned => true;

    [HideLabel]
    public ContainerChooser container = new();

    #region GUI

    protected override void OnInspectorInit()
    {

        base.OnInspectorInit();

        container ??= new();
        container.SetDefaultContainerID("ParticleContainer");

#if UNITY_EDITOR
        AddPrefab(new RegisteredParticle()
        {
            id = "pixel_destroy",
            name = new()
            {
                {"Chinese", "像素破碎"},
                {"English","Pixel Destroy"}
            },
            enableDurationLimitation = true,
            duration = 3,
            particlePrefab = "PixelDestroy".FindAssetOfName<ParticleSystem>()
        });
#endif
    }

    #endregion


}

public class RegisteredParticle : 
    GamePrefabCoreBundle<RegisteredParticle, ParticleSpawnerGeneralSetting>.GameItemPrefab
{
    [LabelText("粒子预制体")]
    [AssetList]
    [AssetSelector(Paths = "Assets")]
    [AssetsOnly]
    [Required]
    public ParticleSystem particlePrefab;

    [LabelText("持续时间限制")]
    [ToggleButtons("是", "否")]
    public bool enableDurationLimitation = false;

    [LabelText("持续时间")]
    [ShowIf(nameof(enableDurationLimitation))]
    public FloatSetter duration = new();

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        duration ??= new();
    }
}