using Basis;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using System.Runtime.CompilerServices;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public abstract class GeneralSettingBase : GameSettingBase
{
    public static StringTranslation coreCategory = new()
    {
        { "English", "Core" },
        { "Chinese", "核心" }
    };

    public static StringTranslation builtInCategory = new()
    {
        { "English", "Built-in" },
        { "Chinese", "内置模块" }
    };

    public static StringTranslation visualEffectCategory = new()
    {
        { "English", "Visual Effect" },
        { "Chinese", "特效" }
    };

    public static StringTranslation settingSuffixName = new()
    {
        { "English", "General Setting" },
        { "Chinese", "通用设置" }
    };

    /// <summary>
    /// 设置的名字
    /// </summary>
    public virtual StringTranslation settingName => new()
    {
        { "English", "Unregistered" },
        { "Chinese", "未注册" }
    };

    public virtual string fullSettingName => settingName + settingSuffixName;

    public virtual bool ignoreGeneralSettingsInGameEditor => false;

    /// <summary>
    /// 在总编辑器中的文件夹路径，默认为空
    /// </summary>
    public virtual StringTranslation folderPath => "";

    /// <summary>
    /// 是否在总编辑器里有图标
    /// </summary>
    public virtual bool hasIcon => false;

    /// <summary>
    /// 在总编辑器里显示的图标，需要hasIcon为true
    /// </summary>
    public virtual SdfIconType icon => SdfIconType.Play;

    [LabelText("使用特定的JSON存储文件"), Space(10)]
    [GUIColor(0.906f, 0.635f, 0.227f)]
    [ToggleButtons("是", "否"), PropertyOrder(-1000)]
    public bool useSpecificJsonFile = false;

    [LabelText("存储JSON文件")]
    [GUIColor(0.906f, 0.635f, 0.227f)]
    [Required, ShowIf(nameof(useSpecificJsonFile)), PropertyOrder(-998)]
    [AssetSelector(Filter = "GeneralSetting")]
    public TextAsset jsonFile;

    [FormerlySerializedAs("jsonFolderRelativePath")]
    [LabelText(@"@settingName + ""设置数据文本存储路径""")]
    [FolderPath(ParentFolder = "Assets"), PropertyOrder(-999)]
    [GUIColor(0.906f, 0.635f, 0.227f)]
    [InfoBox("请选择设置相对路径", InfoMessageType.Warning,
        @"@StringFunc.IsEmptyAfterTrim(dataFolderRelativePath)")]
    [InfoBox(@"@""绝对路径:"" + dataFolderPath",
        VisibleIf = @"@StringFunc.IsEmptyAfterTrim(dataFolderRelativePath) == false")]
    [HideIf(nameof(useSpecificJsonFile))]
    [OnInspectorInit(nameof(OnInspectorInit))]
    public string dataFolderRelativePath = "Configurations";

    [LabelText("默认JSON文件后缀")]
    [GUIColor(0.906f, 0.635f, 0.227f)]
    [InfoBox("不能为空", InfoMessageType.Error, "@StringFunc.IsEmptyAfterTrim($value)")]
    [HideIf(nameof(useSpecificJsonFile)), PropertyOrder(-997)]
    public string defaultJSONFileSuffix = "txt";

    [LabelText("数据存储文件夹")]
    [GUIColor(0.906f, 0.635f, 0.227f)]
    [ShowIf(nameof(useSpecificJsonFile))]
    [ShowInInspector, DisplayAsString, PropertyOrder(-996)]
    public string dataFolderPath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
#if UNITY_EDITOR
            if (useSpecificJsonFile)
            {
                return jsonFilePath.GetDirectoryPath();
            }
#endif
            
            return IOFunc.assetsFolderPath.PathCombine(dataFolderRelativePath);
        }
    }

    [LabelText("JSON存储路径")]
    [GUIColor(0.906f, 0.635f, 0.227f)]
    [ShowIf(nameof(useSpecificJsonFile))]
    [ShowInInspector, DisplayAsString, PropertyOrder(-996)]
    public string jsonFilePath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {

#if UNITY_EDITOR
            if (useSpecificJsonFile)
            {
                return jsonFile.GetAssetAbsolutePath();
            }
#endif

            return dataFolderPath.PathCombine(
                $"{settingName.GetTranslation("English")}GeneralSetting.{defaultJSONFileSuffix}");
        }
    }

    #region GUI

    protected virtual void OnInspectorInit()
    {

    }

    #endregion

    #region JSON

    private string GetJSONString()
    {
        JsonSerializer serializer = new();

        serializer.Converters.AddRange(CustomJSONConverter.converters);

        var o = JObject.FromObject(this, serializer);

        string json = JsonConvert.SerializeObject(o,
            Formatting.Indented,
            CustomJSONConverter.converters);

        return json;
    }

    [Button("打开数据存储文件夹"), PropertyOrder(-990)]
    [PropertySpace(0, 10)]
    [GUIColor(0.906f, 0.635f, 0.227f)]
    private void OpenDataStorageFolder()
    {
        dataFolderPath.OpenDirectory();
    }

    [TitleGroup("JSON", Order = 500)]
    [Button("写入JSON文件")]
    public void WriteToJSON()
    {
        if (dataFolderRelativePath.IsNullOrEmptyAfterTrim())
        {
            Note.note.Error($"相对路径为空，无法写入JSON文件");
        }

        dataFolderPath.CreateDirectory();

        string json = GetJSONString();

        jsonFilePath.OverWriteFile(json);
    }

    [TitleGroup("JSON", Order = 500)]
    [Button("读取JSON文件")]
    public void ReadFromJSON()
    {
        if (dataFolderRelativePath.IsNullOrEmptyAfterTrim())
        {
            Note.note.Error($"相对路径为空，无法读取JSON文件");
        }

        if (jsonFilePath.ExistsFile() == false)
        {
            Note.note.Error("不存在指定的JSON文件，请先创建JSON文件或写入JSON文件");
        }

        string json = jsonFilePath.ReadText();

        var o = JObject.Parse(json, new JsonLoadSettings()
        {

        });

        foreach (var fieldInfo in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            if (fieldInfo.HasAttribute<JsonIgnoreAttribute>())
            {
                continue;
            }

            var jsonProperty = fieldInfo.GetAttribute<JsonPropertyAttribute>();

            if (jsonProperty != null)
            {
                string propertyName = fieldInfo.Name;

                if (jsonProperty.PropertyName.IsNullOrEmptyAfterTrim() == false)
                {
                    propertyName = jsonProperty.PropertyName;
                }

                Debug.Log(propertyName);

                var serializer = new JsonSerializer();

                serializer.Converters.AddRange(CustomJSONConverter.converters);

                try
                {
                    if (o.TryGetValue(propertyName, out JToken value))
                    {
                        fieldInfo.SetValue(
                            this,
                            serializer.Deserialize(value.CreateReader(), fieldInfo.FieldType)
                        );
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                    throw;
                }
                
            }
        }
    }

    [TitleGroup("JSON", Order = 500)]
    [Button("打开JSON文件")]
    public void OpenJSON()
    {
        jsonFilePath.OpenFile();
    }

    #endregion
}
