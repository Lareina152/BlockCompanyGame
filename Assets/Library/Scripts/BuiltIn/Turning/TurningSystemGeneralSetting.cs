using Basis;
using System.Collections;
using System.Collections.Generic;
using Basis.GameItem;
using UnityEngine;

[CreateAssetMenu(fileName = "TurningSystemGeneralSetting", menuName = "GameConfiguration/TurningSystemGeneralSetting")]
public class TurningSystemGeneralSetting : 
    GamePrefabCoreBundle<TurningSystemConfiguration, TurningSystemGeneralSetting>.GameItemGeneralSetting
{
    public override StringTranslation settingName => new()
    {
        { "English", "TurningSystem" },
        { "Chinese", "回合系统" }
    };

    public override string fullSettingName => settingName;

    public override StringTranslation folderPath => builtInCategory;

    public override StringTranslation prefabName => new()
    {
        { "English", "TurningSystem" },
        { "Chinese", "回合系统" }
    };

    public override StringTranslation prefabSuffixName => new()
    {
        { "English", "Preset" },
        { "Chinese", "预设" }
    };

    public override bool gameTypeAbandoned => true;
}
