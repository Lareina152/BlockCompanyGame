using Basis;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#if UNITY_EDITOR
using ToDoList;
#endif

using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameCoreSettingBase : UniqueGameSettingBaseInScene<GameCoreSettingBase>
{
    [ShowInInspector, ReadOnly]
    protected static GameCoreSettingBaseFile _gameCoreSettingsFileBase;

    public static GameCoreSettingBaseFile gameCoreSettingsFileBase
    {
        get
        {
            if (_gameCoreSettingsFileBase == null)
            {
                LoadGameSettingFile();
            }

            return _gameCoreSettingsFileBase;
        }
    }

#if UNITY_EDITOR
    public static TextureImportTools textureImportTools =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            textureImportTools;

    public static ToDoListTools toDoListTools =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            toDoListTools;

    public static ColorfulHierarchyGeneralSetting colorfulHierarchyGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            colorfulHierarchyGeneralSetting;
#endif

    public static ManagerCreationGeneralSetting managerCreationGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            managerCreationGeneralSetting;

    public static ProcedureGeneralSetting procedureGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            procedureGeneralSetting;

    public static TranslationGeneralSetting translationGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            translationGeneralSetting;

    public static string currentLanguage
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (gameCoreSettingsFileBase == null)
            {
                return null;
            }

            if (translationGeneralSetting == null)
            {
                return null;
            }

            return translationGeneralSetting.currentLanguage;
        }
    }

    public static GlobalEventSystemGeneralSetting globalEventSystemGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            globalEventSystemGeneralSetting;

    public static MouseEventGeneralSetting mouseEventGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            mouseEventGeneralSetting;

    public static ColorGeneralSetting colorGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            colorGeneralSetting;

    public static UpdateDelegateGeneralSetting updateDelegateGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            updateDelegateGeneralSetting;

    public static ObjectTracerGeneralSetting objectTracerGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            objectTracerGeneralSetting;

    public static NoteGeneralSetting noteGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            noteGeneralSetting;


    public static ParticleSpawnerGeneralSetting particleSpawnerGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            particleSpawnerGeneralSetting;

    public static TrailSpawnerGeneralSetting trailSpawnerGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            trailSpawnerGeneralSetting;


    public static UIParticleGeneralSetting uiParticleGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            uiParticleGeneralSetting;


    public static MapCoreGeneralSetting mapCoreGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            mapCoreGeneralSetting;

    public static ExtRuleTileGeneralSetting extRuleTileGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            extRuleTileGeneralSetting;

    public static PropertyGeneralSetting propertyGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            propertyGeneralSetting;

    public static TurningSystemGeneralSetting turningSystemGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            turningSystemGeneralSetting;

    public static UIPanelGeneralSetting uiPanelGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            uiPanelGeneralSetting;

    public static DebugUIPanelGeneralSetting debugUIPanelGeneralSetting =>
        gameCoreSettingsFileBase == null ? null : gameCoreSettingsFileBase.
            debugUIPanelGeneralSetting;

    public new static void Init()
    {
        gameCoreSettingsFileBase.Init();
    }

    public static GameCoreSettingBaseFile LoadGameSettingFile()
    {
        _gameCoreSettingsFileBase = 
            Resources.Load<GameCoreSettingBaseFile>("Configurations/GameSetting");

        if (_gameCoreSettingsFileBase == null)
        {
            Debug.LogError($"未在默认路径中找到游戏总设置");
            return null;
        }

        return _gameCoreSettingsFileBase;
    }

    public static Type GetExtendedCoreSettingType()
    {
        var extendedCoreSettingType = typeof(GameCoreSettingBase).
            FindDerivedClasses(false, false).FirstOrDefault();

        extendedCoreSettingType ??= typeof(GameCoreSettingBase);

        return extendedCoreSettingType;
    }

    [Button("获取所有通用设置", ButtonStyle.Box), FoldoutGroup("Debugging")]
    public static IReadOnlyList<GeneralSettingBase> GetAllGeneralSettings()
    {
        var allGeneralSettings = new List<GeneralSettingBase>();

        var extendedCoreSettingType = GetExtendedCoreSettingType();

        if (extendedCoreSettingType != null)
        {
            foreach (var generalSetting in extendedCoreSettingType.
                         GetAllStaticPropertyValuesByReturnType(typeof(GeneralSettingBase)))
            {
                allGeneralSettings.Add((GeneralSettingBase)generalSetting);
            }
        }

        return allGeneralSettings;
    }

    [Button("获取通用设置", ButtonStyle.Box), FoldoutGroup("Debugging")]
    public static GeneralSettingBase FindGeneralSetting(Type generalSettingType)
    {
        var extendedCoreSettingType = GetExtendedCoreSettingType();

        return (GeneralSettingBase)extendedCoreSettingType.
            GetAllStaticPropertyValuesByReturnType(generalSettingType).FirstOrDefault();
    }

    public static T FindGeneralSetting<T>(Type generalSettingType)
    {
        var extendedCoreSettingType = GetExtendedCoreSettingType();

        return (T)extendedCoreSettingType.
            GetAllStaticPropertyValuesByReturnType(generalSettingType).FirstOrDefault();
    }

    public static T FindGeneralSetting<T>() where T : GeneralSettingBase
    {
        return (T)FindGeneralSetting(typeof(T));
    }
}
