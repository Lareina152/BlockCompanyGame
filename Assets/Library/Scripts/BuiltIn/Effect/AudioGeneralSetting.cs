using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Basis;
using Basis.GameItem;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class AudioPreset : 
    GamePrefabCoreBundle<AudioPreset, AudioGeneralSetting>.GameItemPrefab
{
    [LabelText("音效片段")]
    [Required]
    public AudioClip audioClip;

    [LabelText("音量")]
    [PropertyRange(0, 1)]
    public float volume = 1;

    [LabelText("自动检查是否停止")]
    public bool autoCheckStop = true;

    public override void CheckSettings()
    {
        base.CheckSettings();

        Note.note.AssertIsNotNull(audioClip, nameof(audioClip));
    }
}

public class AudioGeneralSetting :
    GamePrefabCoreBundle<AudioPreset, AudioGeneralSetting>.GameItemGeneralSetting,
    IManagerCreationProvider
{
    public override StringTranslation settingName => new()
    {
        { "Chinese", "音效" },
        { "English", "Audio" }
    };

    public override StringTranslation prefabName => new()
    {
        { "Chinese", "音效" },
        { "English", "Audio" }
    };

    public override StringTranslation prefabSuffixName => new()
    {
        { "Chinese", "预设" },
        { "English", "Preset" }
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
        container.SetDefaultContainerID("#Audio Container");

    }

    [Button("从选中的预制体注册音效")]
    [ShowIf(nameof(ShowRegisterAudioClipFromSelectionButton))]
    private void RegisterAudioClipFromSelection()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is not AudioClip audioClip)
            {
                continue;
            }

            AddPrefab(new AudioPreset()
            {
                id = obj.name,
                name = obj.name,
                audioClip = audioClip,
                autoCheckStop = true,
                volume = 1
            });
        }
    }

    [Button("从选中的预制体添加注册的粒子（不重复）")]
    [ShowIf(nameof(ShowRegisterAudioClipFromSelectionButton))]
    private void RegisterUniqueAudioClipFromSelection()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is not AudioClip audioClip)
            {
                continue;
            }

            if (GetAllPrefabs().Any(prefab => prefab.audioClip == audioClip))
            {
                Note.note.Warning($"为了放置重复，忽略了{audioClip.name}的添加");
                continue;
            }

            AddPrefab(new AudioPreset()
            {
                id = obj.name,
                name = obj.name,
                audioClip = audioClip,
                autoCheckStop = true,
                volume = 1
            });
        }
    }

    private bool ShowRegisterAudioClipFromSelectionButton()
    {
        return Selection.objects.Any(obj => obj is AudioClip);
    }

    #endregion


    public IEnumerable<ManagerType> GetManagerTypes()
    {
        yield return ManagerType.VFXCore;
    }

    public void HandleManagerCreation(ManagerType managerType, Transform managerContainer)
    {
        if (managerType == ManagerType.VFXCore)
        {
            managerContainer.GetOrAddComponent<AudioSpawner>();
        }
    }
}
