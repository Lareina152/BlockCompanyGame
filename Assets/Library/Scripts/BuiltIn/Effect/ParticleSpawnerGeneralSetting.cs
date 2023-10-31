using Basis;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Basis.GameItem;
using Basis.Utility;
using ConfigurationBasis;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

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

public class ParticleSpawnerGeneralSetting : 
    GamePrefabCoreBundle<RegisteredParticle, ParticleSpawnerGeneralSetting>.GameItemGeneralSetting,
    IManagerCreationProvider
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
        container.SetDefaultContainerID("@Particle Container");
    }

    [Button("从选中的预制体添加注册的粒子")]
    [ShowIf(nameof(ShowRegisterParticleFromSelectionButton))]
    private void RegisterParticleFromSelection()
    {
        foreach (var gameObject in Selection.gameObjects)
        {
            if (gameObject.IsPrefabAsset() == false)
            {
                continue;
            }

            var particleSystem = gameObject.GetComponentInChildren<ParticleSystem>();

            if (particleSystem == null)
            {
                continue;
            }

            var enableDurationLimitation = particleSystem.main.loop == false;

            var duration = particleSystem.main.duration;

            AddPrefab(new RegisteredParticle()
            {
                id = gameObject.name,
                name = gameObject.name,
                particlePrefab = particleSystem,
                enableDurationLimitation = enableDurationLimitation,
                duration = duration
            });
        }
    }

    [Button("从选中的预制体添加注册的粒子（不重复）")]
    [ShowIf(nameof(ShowRegisterParticleFromSelectionButton))]
    private void RegisterUniqueParticleFromSelection()
    {
        foreach (var gameObject in Selection.gameObjects)
        {
            if (gameObject.IsPrefabAsset() == false)
            {
                continue;
            }

            var particleSystem = gameObject.GetComponentInChildren<ParticleSystem>();

            if (particleSystem == null)
            {
                continue;
            }

            if (GetAllPrefabs().Any(prefab => prefab.particlePrefab == particleSystem))
            {
                Note.note.Warning($"为了放置重复，忽略了{gameObject.name}的添加");
                continue;
            }

            var enableDurationLimitation = particleSystem.main.loop == false;

            var duration = particleSystem.main.duration;

            AddPrefab(new RegisteredParticle()
            {
                id = gameObject.name,
                name = gameObject.name,
                particlePrefab = particleSystem,
                enableDurationLimitation = enableDurationLimitation,
                duration = duration
            });
        }
    }

    private bool ShowRegisterParticleFromSelectionButton()
    {
        return Selection.gameObjects.Any(obj => obj.IsPrefabAsset() &&
                                                obj.HasComponent<ParticleSystem>());
    }

    #endregion

    [Button("设置持续时间")]
    public void SetDuration(
        [ValueDropdown("@GetPrefabNameList()")]
        string id, float duration)
    {
        var prefab = GetPrefabStrictly(id);
        
        foreach (var particleSystem in prefab.particlePrefab.
                     FindComponentsInChildren<ParticleSystem>(true))
        {
            var main = particleSystem.main;
            main.duration = duration;
        }
    }


    public IEnumerable<ManagerType> GetManagerTypes()
    {
        yield return ManagerType.VFXCore;
    }

    public void HandleManagerCreation(ManagerType managerType, Transform managerContainer)
    {
        if (managerType == ManagerType.VFXCore)
        {
            managerContainer.GetOrAddComponent<ParticleSpawner>();
        }
    }
}