using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : GameCoreSettingBase
{
    public static GameSettingFile gameSettingFile => (GameSettingFile)gameCoreSettingsFileBase;

    public static CameraGeneralSetting cameraGeneralSetting => 
        gameSettingFile.cameraGeneralSetting;
}
