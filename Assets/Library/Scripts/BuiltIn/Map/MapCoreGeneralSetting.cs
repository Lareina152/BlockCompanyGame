using Basis;
using MapBasis;
using System.Collections;
using System.Collections.Generic;
using Basis.GameItem;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "MapCoreGeneralSetting", menuName = "GameConfiguration/MapCoreGeneralSetting")]
public class MapCoreGeneralSetting : 
    GamePrefabCoreBundle<MapConfiguration, MapCoreGeneralSetting>.GameItemGeneralSetting
{
    public override StringTranslation settingName => new()
    {
        { "English", "MapCore" },
        { "Chinese", "地图核心" }
    };

    public override string fullSettingName => settingName;

    public override bool hasIcon => true;

    public override SdfIconType icon => SdfIconType.PinMap;

    public override StringTranslation folderPath => builtInCategory;

    public override StringTranslation prefabName => new()
    {
        { "English", "MapCore" },
        { "Chinese", "地图核心" }
    };

    public override bool gameTypeAbandoned => true;
}
