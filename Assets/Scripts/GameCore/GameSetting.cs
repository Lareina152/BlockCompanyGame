using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : GameCoreSettingBase
{
    public static GameSettingFile gameSettingFile => (GameSettingFile)gameCoreSettingsFileBase;

    public static CameraGeneralSetting cameraGeneralSetting => 
        gameSettingFile.cameraGeneralSetting;

    public static EntityGeneralSetting entityGeneralSetting => 
        gameSettingFile.entityGeneralSetting;

    public static ItemGeneralSetting itemGeneralSetting =>
        gameSettingFile.itemGeneralSetting;

    public static StageGeneralSetting stageGeneralSetting => 
        gameSettingFile.stageGeneralSetting;
}
