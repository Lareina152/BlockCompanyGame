using System;
using ConfigurationBasis;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Basis;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class LabelGUIFormat : ValuePreviewBase<string>
{
    protected override bool valueAlwaysPreviewed => true;

    protected override bool showPreviewValueBelow => false;

    [LabelText("覆盖字体风格")]
    [OnValueChanged(nameof(PreviewValue))]
    public bool overrideFontStyle = false;

    [LabelText("粗体")]
    [Indent]
    [JsonProperty]
    [OnValueChanged(nameof(PreviewValue))]
    [ShowIf(nameof(overrideFontStyle))]
    public bool isBold = false;

    [LabelText("斜体")]
    [Indent]
    [JsonProperty]
    [OnValueChanged(nameof(PreviewValue))]
    [ShowIf(nameof(overrideFontStyle))]
    public bool isItalic = false;

    [LabelText("覆盖字体大小")]
    [OnValueChanged(nameof(PreviewValue))]
    public bool overrideFontSize = false;

    [LabelText("字体大小")]
    [Indent]
    [JsonProperty]
    [OnValueChanged(nameof(PreviewValue))]
    [ShowIf(nameof(overrideFontSize))]
    public int fontSize;

    [LabelText("覆盖字体颜色")]
    [OnValueChanged(nameof(PreviewValue))]
    public bool overrideFontColor = false;

    [LabelText("字体颜色")]
    [Indent]
    [ColorPalette]
    [JsonProperty]
    [OnValueChanged(nameof(PreviewValue))]
    [ShowIf(nameof(overrideFontColor))]
    public Color fontColor = Color.black;

    public void Set(Label label)
    {
        if (overrideFontStyle)
        {
            var fontStyle = isBold switch
            {
                true when isItalic => FontStyle.BoldAndItalic,
                true when isItalic == false => FontStyle.Bold,
                false when isItalic => FontStyle.Italic,
                false when isItalic == false => FontStyle.Normal,
                _ => throw new ArgumentException()
            };
            label.style.unityFontStyleAndWeight = fontStyle;
        }

        if (overrideFontSize)
        {
            label.style.fontSize = fontSize;
        }

        if (overrideFontColor)
        {
            label.style.color = fontColor;
        }
    }

    protected override void PreviewValue()
    {
        var strList = new List<string>();

        if (overrideFontColor)
        {
            strList.Add(fontColor.ToString(ColorStringFormat.Name));
        }

        if (overrideFontSize)
        {
            strList.Add(fontSize.ToString());
        }

        if (overrideFontStyle)
        {
            if (isBold)
            {
                strList.Add("粗体");
            }

            if (isItalic)
            {
                strList.Add("斜体");
            }
        }

        if (strList.Count == 0)
        {
            contentPreviewing = "无格式覆盖";
        }
        else
        {
            contentPreviewing = strList.ToString(",");
        }
    }
}
