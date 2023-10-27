using Basis;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using ToDoList;
#endif
using UnityEditor;
using UnityEngine;

public class GameCoreSettingBaseFile : GameSettingBase, IManagerCreationProvider
{
    public const string TOOLS_SETTING_CATEGORY = "工具";
    public const string CORE_SETTING_CATEGORY = "核心";
    public const string EFFECT_SETTING_CATEGORY = "特效";
    public const string BUILTIN_MODULE_SETTING_CATEGORY = "内置模块";
    public const string UI_SETTING_CATEGORY = "UI";

    public const string defaultGeneralSettingDirectoryPath = "Assets/Resources/Configurations/GeneralSetting";
    public const string defaultGameCoreSettingFilePath = "Assets/Resources/Configurations";
    public const string defaultName = "GameSetting";

    public override bool isSettingUnmovable => true;

    public override string forcedFileName => "GameSetting";

    public virtual Type thisType => GetType();

    [Space(10)]
    [LabelText("通用设置存放文件夹")]
    [PropertyOrder(-999)]
    [FolderPath]
    public string generalSettingDirectoryPath = defaultGeneralSettingDirectoryPath;

#if UNITY_EDITOR
    [LabelText("图片导入工具"), BoxGroup(TOOLS_SETTING_CATEGORY)]
    [Required]
    public TextureImportTools textureImportTools;

    [LabelText("待办清单"), BoxGroup(TOOLS_SETTING_CATEGORY)]
    [Required]
    public ToDoListTools toDoListTools;

    [LabelText("带颜色的层级"), BoxGroup(TOOLS_SETTING_CATEGORY)]
    [Required]
    public ColorfulHierarchyGeneralSetting colorfulHierarchyGeneralSetting;
#endif

    [LabelText("管理器创建设置"), BoxGroup(CORE_SETTING_CATEGORY)]
    [Required]
    public ManagerCreationGeneralSetting managerCreationGeneralSetting;

    [LabelText("流程设置"), BoxGroup(CORE_SETTING_CATEGORY)]
    [Required]
    public ProcedureGeneralSetting procedureGeneralSetting;

    [LabelText("语言通用设置"), BoxGroup(CORE_SETTING_CATEGORY)]
    [Required]
    public TranslationGeneralSetting translationGeneralSetting;

    [LabelText("全局事件通用设置"), BoxGroup(CORE_SETTING_CATEGORY)]
    [Required]
    public GlobalEventSystemGeneralSetting globalEventSystemGeneralSetting;

    [LabelText("鼠标事件通用设置"), BoxGroup(CORE_SETTING_CATEGORY)]
    [Required]
    public MouseEventGeneralSetting mouseEventGeneralSetting;

    [LabelText("颜色通用设置"), BoxGroup(CORE_SETTING_CATEGORY)]
    [Required]
    public ColorGeneralSetting colorGeneralSetting;

    [LabelText("更新代理通用设置"), BoxGroup(CORE_SETTING_CATEGORY)]
    [Required]
    public UpdateDelegateGeneralSetting updateDelegateGeneralSetting;

    [LabelText("追踪器通用设置"), BoxGroup(CORE_SETTING_CATEGORY)]
    [Required]
    public ObjectTracerGeneralSetting objectTracerGeneralSetting;

    [LabelText("日志通用设置"), BoxGroup(CORE_SETTING_CATEGORY)]
    [Required]
    public NoteGeneralSetting noteGeneralSetting;

    [LabelText("粒子生成器设置"), BoxGroup(EFFECT_SETTING_CATEGORY)]
    [Required]
    public ParticleSpawnerGeneralSetting particleSpawnerGeneralSetting;

    [LabelText("粒子生成器设置"), BoxGroup(EFFECT_SETTING_CATEGORY)]
    [Required]
    public TrailSpawnerGeneralSetting trailSpawnerGeneralSetting;

    [LabelText("UI粒子通用设置"), BoxGroup(BUILTIN_MODULE_SETTING_CATEGORY)]
    [Required]
    public UIParticleGeneralSetting uiParticleGeneralSetting;

    [LabelText("地图核心通用设置"), BoxGroup(BUILTIN_MODULE_SETTING_CATEGORY)]
    [Required]
    public MapCoreGeneralSetting mapCoreGeneralSetting;

    [LabelText("拓展瓦片通用设置"), BoxGroup(BUILTIN_MODULE_SETTING_CATEGORY)]
    [Required]
    public ExtRuleTileGeneralSetting extRuleTileGeneralSetting;

    [LabelText("属性通用设置"), BoxGroup(BUILTIN_MODULE_SETTING_CATEGORY)]
    [Required]
    public PropertyGeneralSetting propertyGeneralSetting;

    [LabelText("回合系统通用设置"), BoxGroup(BUILTIN_MODULE_SETTING_CATEGORY)]
    [Required]
    public TurningSystemGeneralSetting turningSystemGeneralSetting;

    [LabelText("UI通用设置"), BoxGroup(UI_SETTING_CATEGORY)]
    [Required]
    public UIPanelGeneralSetting uiPanelGeneralSetting;

    [LabelText("Debug UI面板通用设置"), BoxGroup(UI_SETTING_CATEGORY)]
    [Required]
    public DebugUIPanelGeneralSetting debugUIPanelGeneralSetting;

    #region GUI

#if UNITY_EDITOR

    protected override void Awake()
    {
        base.Awake();

        AutoFindSetting();
    }

    [Button("重置路径")]
    [ShowIf("@generalSettingDirectoryPath != defaultGeneralSettingDirectoryPath")]
    [PropertyOrder(-998)]
    public void ResetGeneralSettingDirectoryPath()
    {
        generalSettingDirectoryPath = defaultGeneralSettingDirectoryPath;
    }

    [Button("自动寻找并创建通用配置文件", ButtonSizes.Medium), TitleGroup(DEBUGGING_CATEGORY)]
    private void AutoFindSettingAndCreate()
    {
        foreach (var fieldInfo in thisType.GetFields())
        {
            if (fieldInfo.IsPublic && fieldInfo.FieldType.IsDerivedFrom<ScriptableObject>())
            {
                var result =
                    fieldInfo.FieldType.FindOrCreateScriptableObject(generalSettingDirectoryPath, fieldInfo.FieldType.Name);

                fieldInfo.SetValue(this, result);
            }
        }

        EditorUtility.SetDirty(this);

        if (EditorApplication.isUpdating == false)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    [Button("自动寻找通用配置文件", ButtonSizes.Medium), TitleGroup(DEBUGGING_CATEGORY)]
    private void AutoFindSetting()
    {
        foreach (var fieldInfo in thisType.GetFields())
        {
            if (fieldInfo.IsPublic && fieldInfo.FieldType.IsDerivedFrom<ScriptableObject>())
            {
                var result =
                    fieldInfo.FieldType.FindScriptableObject();

                if (result != null)
                {
                    fieldInfo.SetValue(this, result);
                }
            }
        }

        EditorUtility.SetDirty(this);

        if (EditorApplication.isUpdating == false)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public override void CheckSettingsGUI()
    {
        AutoFindSetting();

        base.CheckSettingsGUI();
    }

#endif

    #endregion

    protected override void OnInit()
    {
        base.OnInit();

        CheckSettings();

        var generalSettings = GameCoreSettingBase.GetAllGeneralSettings();

        generalSettings.Examine(setting => setting.PreInit());
        generalSettings.Examine(setting => setting.Init());
        generalSettings.Examine(setting => setting.PostInit());
        generalSettings.Examine(setting => setting.FinishInit());
    }

    public override void CheckSettings()
    {
        foreach (var propertyInfo in GameCoreSettingBase.GetExtendedCoreSettingType().
                     GetAllStaticPropertiesByReturnType(typeof(GeneralSettingBase)))
        {
            var generalSetting = propertyInfo.GetValue(null) as GeneralSettingBase;

            if (generalSetting == null)
            {
                Note.note.Error($"{propertyInfo.Name}不能为Null");
                continue;
            }

            generalSetting.CheckSettings();
        }
    }

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
#endif
    public static void CheckGlobal()
    {
#if UNITY_EDITOR
        var derivedClasses = typeof(GameCoreSettingBaseFile).FindDerivedClasses(false, false).ToList();

        switch (derivedClasses.Count)
        {
            case 0:
                typeof(GameCoreSettingBaseFile).FindOrCreateScriptableObject(defaultGameCoreSettingFilePath, defaultName);
                break;
            case > 2:
                Debug.LogWarning($"不允许有多个类继承{typeof(GameCoreSettingBaseFile)}，" +
                                 $"继承的类如下：{derivedClasses.ToString(",")}");
                derivedClasses[0].FindOrCreateScriptableObject(defaultGameCoreSettingFilePath, defaultName);
                break;
            case 1:
                derivedClasses[0].FindOrCreateScriptableObject(defaultGameCoreSettingFilePath, defaultName);
                break;

        }
#endif
    }

    public IEnumerable<ManagerType> GetManagerTypes()
    {
        yield return ManagerType.SettingCore;
    }

    public void HandleManagerCreation(ManagerType managerType, Transform managerContainer)
    {
        managerContainer.GetOrAddComponent(GameCoreSettingBase.GetExtendedCoreSettingType());
    }
}
