using System;
using Basis;
using System.Collections;
using System.Collections.Generic;
using Basis.GameItem;
using Sirenix.OdinInspector;
using UnityEngine;

public sealed class GlobalEventSystemGeneralSetting : 
    GamePrefabCoreBundle<GlobalEventConfig, GlobalEventSystemGeneralSetting>.GameItemGeneralSetting, 
    IManagerCreationProvider
{
    public override StringTranslation settingName => new()
    {
        { "Chinese", "全局事件" },
        { "English", "Global Event" }
    };

    public override bool hasIcon => true;

    public override SdfIconType icon => SdfIconType.Dpad;

    public override string fullSettingName => settingName;

    public override StringTranslation folderPath => coreCategory;

    public override StringTranslation prefabName => new()
    {
        { "Chinese", "全局事件" },
        { "English", "Global Event" }
    };

    public override StringTranslation prefabSuffixName => new()
    {
        { "Chinese", "设置" },
        { "English", "Config" }
    };

    public override bool gameTypeAbandoned => true;

    [LabelText("KeyCode翻译")]
    public Dictionary<KeyCode, StringTranslation> keyCodeTranslations = new();

    protected override void OnPostInit()
    {
        base.OnPostInit();

        GlobalEventManager.Init();
    }

    public string GetKeyCodeName(KeyCode keyCode)
    {
        if (keyCodeTranslations.TryGetValue(keyCode, out var translation))
        {
            return translation;
        }
        
        return keyCode.ToString();
    } 

    #region Manager Creation

    public IEnumerable<ManagerType> GetManagerTypes()
    {
        yield return ManagerType.EventCore;
    }

    public void HandleManagerCreation(ManagerType managerType, Transform managerContainer)
    {
        managerContainer.GetOrAddComponent<GlobalEventManager>();
    }

    #endregion
}
