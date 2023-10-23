using Basis;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CameraGeneralSetting : GeneralSettingBase
{
    public override StringTranslation settingName => new()
    {
        { "Chinese", "相机" },
        { "English", "Camera" }
    };

    [LabelText("FOV插值速度")]
    [PropertyRange(0, 1)]
    public float fovLerpSpeed = 0.1f;

    [LabelText("位置插值速度")]
    [PropertyRange(0, 1)]
    public float positionLerpSpeed = 0.1f;
}
