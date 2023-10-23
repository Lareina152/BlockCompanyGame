using ConfigurationBasis;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using Basis;
using NPOI.SS.UserModel;
using System.Runtime.CompilerServices;

[HideDuplicateReferenceBox]
[HideReferenceObjectPicker]
[Serializable]
public class TextTagFormat : ValuePreviewBase<string>
{
    protected override bool valueAlwaysPreviewed => true;

    protected override bool showPreviewValueBelow => false;

    [LabelText("覆盖颜色")]
    [JsonProperty]
    [OnValueChanged(nameof(PreviewValue))]
    public bool overrideFontColor = false;

    [LabelText("字体颜色")]
    [Indent]
    [ColorPalette]
    [JsonProperty]
    [OnValueChanged(nameof(PreviewValue), true)]
    [ShowIf(nameof(overrideFontColor))]
    public Color fontColor = Color.black;

    [LabelText("覆盖字体风格")]
    [OnValueChanged(nameof(PreviewValue))]
    public bool overrideFontStyle = false;

    [LabelText("加粗")]
    [JsonProperty]
    [Indent]
    [ShowIf(nameof(overrideFontStyle))]
    [OnValueChanged(nameof(PreviewValue))]
    public bool isBold = false;

    [LabelText("斜体")]
    [JsonProperty]
    [Indent]
    [ShowIf(nameof(overrideFontStyle))]
    [OnValueChanged(nameof(PreviewValue))]
    public bool isItalic = false;

    #region GUI

    protected override void PreviewValue()
    {
        var strList = new List<string>();

        if (overrideFontColor)
        {
            strList.Add(fontColor.ToString(ColorStringFormat.Name));
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

    #endregion

    public string GetText(string customText)
    {
        string result = customText;

        if (overrideFontColor)
        {
            result = result.ColorTag(fontColor);
        }

        if (overrideFontStyle)
        {
            if (isBold)
            {
                result = result.BoldTag();
            }

            if (isItalic)
            {
                result = result.ItalicTag();
            }
        }

        return result;
    }

    public string GetText(object customText)
    {
        return GetText(customText.ToString());
    }

    #region JSON Serialization

    public bool ShouldSerializecolor()
    {
        return overrideFontColor == true;
    }

    #endregion
}
