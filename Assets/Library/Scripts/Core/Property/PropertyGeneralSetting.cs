using System;
using Basis;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Basis.GameItem;
using Sirenix.OdinInspector;
using UnityEngine;

public class PropertyOfGameItem : 
    SimpleGameItemBundle<PropertyConfig, PropertyGeneralSetting, PropertyOfGameItem>.GameItem
{
    private object _target;

    public object target
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _target;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            var oldTarget = _target;
            _target = value;
            OnTargetChanged(oldTarget, _target);
        }
    }

    public Texture2D icon => origin.icon;

    public event Action<string> OnValueStringChanged;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetValueString() => origin.GetValueString(target);

    protected virtual void OnTargetChanged(object previous, object current)
    {
        
    }

    protected void OnFloatValueChanged(float previous, float current)
    {
        OnValueStringChanged?.Invoke(GetValueString());
    }
}

public class PropertyConfig : 
    SimpleGameItemBundle<PropertyConfig, PropertyGeneralSetting, PropertyOfGameItem>.GameItemPrefab
{
    protected override string preferredIDSuffix => "property";

    [LabelText("目标类型"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [ShowInInspector]
    public virtual Type targetType => typeof(object);

    [LabelText("图标"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [Required]
    public Texture2D icon;

    public virtual string GetValueString(object target) => string.Empty;

    public override void CheckSettings()
    {
        base.CheckSettings();

        if (icon == null)
        {
            Note.note.Warning("缺失图标");
        }
    }
}

public class PropertyGeneralSetting : SimpleGameItemBundle<PropertyConfig, PropertyGeneralSetting, PropertyOfGameItem>.GameItemGeneralSetting
{
    public const string PROPERTY_SETTING_CATEGORY = "属性设置";

    public override StringTranslation settingName => new()
    {
        { "Chinese", "属性" },
        { "English", "Property" }
    };

    public override string fullSettingName => settingName;

    public override bool hasIcon => true;

    public override SdfIconType icon => SdfIconType.JournalText;

    public override StringTranslation folderPath => builtInCategory;

    public override StringTranslation prefabName => new()
    {
        { "Chinese", "属性" },
        { "English", "Property" }
    };

    public override StringTranslation prefabSuffixName => new()
    {
        { "Chinese", "设置" },
        { "English", "Config" }
    };

    [LabelText("属性字典"), TitleGroup(PROPERTY_SETTING_CATEGORY)]
    [ShowInInspector]
    [ReadOnly]
    protected static Dictionary<Type, List<PropertyConfig>> propertyConfigs = new();

    protected override void OnPreInit()
    {
        base.OnPreInit();

        propertyConfigs.Clear();

        foreach (var propertyConfig in GetAllPrefabs())
        {
            if (propertyConfigs.ContainsKey(propertyConfig.targetType) == false)
            {
                propertyConfigs[propertyConfig.targetType] = new();
            }

            propertyConfigs[propertyConfig.targetType].Add(propertyConfig);
        }
    }

    [Button(nameof(GetPropertyConfigs)), TitleGroup(PROPERTY_SETTING_CATEGORY)]
    public static List<PropertyConfig> GetPropertyConfigs(Type targetType)
    {
        var result = new List<PropertyConfig>();

        if (propertyConfigs.Count == 0)
        {
            Note.note.Warning("还没加载属性字典");
            return result;
        }

        foreach (var (type, propertyConfig) in propertyConfigs)
        {
            if (type.IsAssignableFrom(targetType))
            {
                result.AddRange(propertyConfig);
            }
        }

        return result;
    }

    public IEnumerable GetPropertyNameList(Type targetType)
    {
        if (allGameItemPrefabs != null)
        {
            foreach (var prefab in allGameItemPrefabs)
            {
                if (prefab is { isActive: true } && prefab.targetType.IsAssignableFrom(targetType))
                {
                    yield return new ValueDropdownItem<string>(prefab.name, prefab.id);
                }
            }
        }
    }
}
