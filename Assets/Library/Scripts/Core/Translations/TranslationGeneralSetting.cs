using System;
using Basis;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Basis.GameItem;
using UnityEngine;

public class TranslationTypeInfo : 
    GamePrefabCoreBundle<TranslationTypeInfo, TranslationGeneralSetting>.GameItemPrefab
{
    
}

[CreateAssetMenu(fileName = "TranslationGeneralSetting", menuName = "GameConfiguration/TranslationGeneralSetting")]
public class TranslationGeneralSetting : 
    GamePrefabCoreBundle<TranslationTypeInfo, TranslationGeneralSetting>.GameItemGeneralSetting 
{
    public const string LANGUAGE_SETTING_CATEGORY = "语言设置";

    public override StringTranslation settingName => new()
    {
        { "English", "Language" },
        { "Chinese", "语言" }
    };

    public override string fullSettingName => settingName;

    public override bool hasIcon => true;

    public override SdfIconType icon => SdfIconType.Globe;

    public override StringTranslation folderPath => coreCategory;

    public override StringTranslation prefabName => new()
    {
        { "English", "Language" },
        { "Chinese", "语言" }
    };

    public override StringTranslation prefabSuffixName => new()
    {
        { "English", "Preset" },
        { "Chinese", "预设" }
    };

    public override int minPrefabCount => 2;

    protected override bool showSetFirstPrefabIDAndNameToDefaultButton => false;

    public override bool gameTypeAbandoned => true;

    [LabelText("当前语言"), TitleGroup(LANGUAGE_SETTING_CATEGORY)]
    [ValueDropdown(nameof(GetPrefabIDList))]
    [JsonProperty, SerializeField]
    private string _currentLanguage = StringTranslation.ANY_LANGUAGE;

    public string currentLanguage
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _currentLanguage;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => SetCurrentLanguage(value);
    }

    public event Action<string, string> OnCurrentLanguageChanged;

    public override void CheckSettings()
    {
        base.CheckSettings();

        if (currentLanguage.IsNullOrEmptyAfterTrim())
        {
            currentLanguage = StringTranslation.ANY_LANGUAGE;
        }

        if (ContainsID(currentLanguage) == false)
        {
            Note.note.Error($"当前语言不在语言设置字典里");
        }

        CheckAnyLanguage();
    }

    public void CheckAnyLanguage()
    {
        AddPrefabToFirst(new()
        {
            id = StringTranslation.ANY_LANGUAGE,
            name = new()
            {
                { StringTranslation.ANY_LANGUAGE, StringTranslation.ANY_LANGUAGE },
            }
        });

        var anyLanguage = GetPrefab(StringTranslation.ANY_LANGUAGE);

        if (anyLanguage.name.Contains(StringTranslation.ANY_LANGUAGE) == false)
        {
            anyLanguage.name.Add(StringTranslation.ANY_LANGUAGE, StringTranslation.ANY_LANGUAGE);
        }
    }

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        CheckAnyLanguage();
    }

    [Button("设置语言"), TitleGroup(LANGUAGE_SETTING_CATEGORY)]
    public void SetCurrentLanguage(
        [LabelText("新语言")]
        [ValueDropdown("@GetPrefabNameList()")]
        string newCurrentLanguage)
    {
        if (ContainsID(newCurrentLanguage) == false)
        {
            Note.note.Warning($"不存在此语言:{newCurrentLanguage}");
        }

        var oldLanguage = _currentLanguage;

        _currentLanguage = newCurrentLanguage;

        OnCurrentLanguageChanged?.Invoke(oldLanguage, _currentLanguage);
    }
}
