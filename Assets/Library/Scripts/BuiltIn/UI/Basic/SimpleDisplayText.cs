using Basis;
using ConfigurationBasis;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleDisplayText : MonoBehaviourBase
{
    [ChildGameObjectsOnly]
    [LabelText("文本容器"), FoldoutGroup("基本")]
    public TextMeshProUGUI text;


    [LabelText("标签"), FoldoutGroup("标签和单位")]
    public TextWithTag label = "Label";

    [LabelText("单位"), FoldoutGroup("标签和单位")]
    public TextWithTag unit = "";

    [LabelText("值格式"), FoldoutGroup("值")]
    public TextTagFormat valueFormat = new();


    [LabelText("标签数值分隔号"), FoldoutGroup("分隔")]
    public string sepLabelValue = "：";

    [LabelText("值和最大值分隔号"), FoldoutGroup("分隔")]
    public string sepValueMaxValue = "/";

    [LabelText("值和单位分隔号"), FoldoutGroup("分隔")]
    public string sepValueUnit = "";

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        label ??= new();
        unit ??= new();
        valueFormat ??= new();
    }

    [Button("预览", ButtonSizes.Medium)]
    protected void SetValue(string value)
    {
        string result = valueFormat.GetText(value);

        if (label.IsEmpty() == false)
        {
            result = label.GetText() + sepLabelValue + result;
        }

        if (unit.IsEmpty() == false)
        {
            result += sepValueUnit + unit.GetText();
        }


        text.text = result;
    }
}
