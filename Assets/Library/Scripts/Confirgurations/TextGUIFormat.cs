using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using ConfigurationBasis;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

[Flags]
public enum FontStyles
{
    [LabelText("普通")]
    Normal = 0x0,
    [LabelText("加粗")]
    Bold = 0x1,
    [LabelText("斜体")]
    Italic = 0x2,
    [LabelText("下划线")]
    Underline = 0x4,
    [LabelText("小写")]
    LowerCase = 0x8,
    [LabelText("大写")]
    UpperCase = 0x10,
    [LabelText("小型大写字母")]
    SmallCaps = 0x20,
    [LabelText("删除线")]
    Strikethrough = 0x40,
    [LabelText("上标")]
    Superscript = 0x80,
    [LabelText("下标")]
    Subscript = 0x100,
    [LabelText("高亮")]
    Highlight = 0x200,
    [LabelText("全选")]
    All = 0x3ff,
};

[HideDuplicateReferenceBox]
[HideReferenceObjectPicker]
[Serializable]
public class TextGUIFormat : ValuePreviewBase<string>
{
    protected override bool valueAlwaysPreviewed => true;

    [LabelText("字体颜色")]
    [ColorPalette]
    [JsonProperty]
    [OnValueChanged(nameof(PreviewValue))]
    public Color fontColor;

    [LabelText("字体格式")]
    [JsonProperty]
    [OnValueChanged(nameof(PreviewValue))]
    public FontStyles fontStyles;

    [LabelText("字体大小")]
    [JsonProperty]
    [OnValueChanged(nameof(PreviewValue))]
    public int fontSize;

    public void Set(TextMeshProUGUI textMeshProUGUI)
    {
        textMeshProUGUI.color = fontColor;
        textMeshProUGUI.fontStyle = (TMPro.FontStyles)fontStyles;
        textMeshProUGUI.fontSize = fontSize;
    }

    protected override void PreviewValue()
    {
        contentPreviewing = $"{fontColor.ToString(ColorStringFormat.Name)},{fontSize}";
    }
}
