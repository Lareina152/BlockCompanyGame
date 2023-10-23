using Basis;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectTracerGeneralSetting", menuName = "GameConfiguration/ObjectTracerGeneralSetting")]
public class ObjectTracerGeneralSetting : GeneralSettingBase
{
    public override StringTranslation settingName => new()
    {
        { "Chinese", "追踪器" },
        { "English", "Object Tracer" }
    };

    public override StringTranslation folderPath => coreCategory;

    [LabelText("调试模式")]
    [ToggleButtons("开", "关")]
    public bool isDebugging = false;
}
