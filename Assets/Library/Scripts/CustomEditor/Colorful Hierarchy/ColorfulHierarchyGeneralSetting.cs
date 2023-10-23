#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ColorPreset
{
    [StringIsNotNullOrEmpty]
    [LabelText("关键字符")]
    public string keyChar;
    [LabelText("文字颜色")]
    public Color textColor = Color.white;
    [LabelText("背景颜色")]
    public Color backgroundColor = Color.black;
    [LabelText("文字对齐")]
    [EnumToggleButtons]
    public TextAnchor textAlignment = TextAnchor.MiddleCenter;
    [LabelText("字体格式")]
    [EnumToggleButtons]
    public FontStyle fontStyle = FontStyle.Bold;
    [HideLabel]
    [ToggleButtons("自动大写字母", "保持原样")]
    public bool autoUpperLetters = true;
}

public class ColorfulHierarchyGeneralSetting : GeneralSettingBase
{
    public override StringTranslation settingName => new()
        {
        { "Chinese", "彩色层级" },
        { "English", "Colorful Hierarchy" }
    };

    public override StringTranslation folderPath => builtInCategory;

    public override bool hasIcon => true;

    public override SdfIconType icon => SdfIconType.FileEarmarkRichtextFill;

    [LabelText("颜色预设")]
    public List<ColorPreset> colorPresets = new List<ColorPreset>();
}

#endif