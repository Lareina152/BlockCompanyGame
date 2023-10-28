using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GameSetting", menuName = "GameConfiguration/GameSetting")]
public class GameSettingFile : GameCoreSettingBaseFile
{
    public override Type thisType => GetType();

    public CameraGeneralSetting cameraGeneralSetting;

    public EntityGeneralSetting entityGeneralSetting;

    public ItemGeneralSetting itemGeneralSetting;

    public StageGeneralSetting stageGeneralSetting;
}
