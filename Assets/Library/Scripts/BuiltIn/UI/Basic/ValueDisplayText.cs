using System.Collections;
using System.Collections.Generic;
using Basis;
using ConfigurationBasis;
using Sirenix.OdinInspector;
using TMPro;

public class ValueDisplayText : ValueDisplayUIBase
{
    [ChildGameObjectsOnly]
    [LabelText("文本容器"), FoldoutGroup("基本")]
    public TextMeshProUGUI text;


    [LabelText("标签"), FoldoutGroup("标签和单位")]
    public TextWithTag label = "Label";

    [LabelText("单位"), FoldoutGroup("标签和单位")]
    public TextWithTag unit = "";


    [LabelText("显示最大值"), FoldoutGroup("值")]
    public bool showMaxValue = true;

    [LabelText("用百分比"), FoldoutGroup("值")]
    public bool percentFormat = false;

    [LabelText("值格式"), FoldoutGroup("值")]
    public TextTagFormat valueFormat = new();


    [LabelText("标签数值分隔号"), FoldoutGroup("分隔")]
    public string sepLabelValue = "：";

    [LabelText("值和最大值分隔号"), FoldoutGroup("分隔")]
    public string sepValueMaxValue = "/";

    [LabelText("值和单位分隔号"), FoldoutGroup("分隔")]
    public string sepValueUnit = "";

    private void Start()
    {
        OnValueChanged();
    }

    #region GUI

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        label ??= new();
        unit ??= new();
        valueFormat ??= new();
    }

    #endregion

    [Button("预览", ButtonSizes.Medium)]
    protected override void OnValueChanged()
    {
        base.OnValueChanged();

        string valueText = percentFormat ? percentString : valueString;
        string maxValueText = percentFormat ? "100" : maxValueString;

        if (showMaxValue)
        {
            valueText += $"{sepValueMaxValue}{maxValueText}";
            if (percentFormat)
            {
                valueText += "%";
            }
        }

        string result = valueFormat.GetText(valueText);

        if (label.IsEmpty() == false)
        {
            result = label.GetText(percent) + sepLabelValue + result;
        }

        if (unit.IsEmpty() == false)
        {
            result += sepValueUnit + unit.GetText(percent);
        }


        text.text = result;
    }

    [Button("设置最大值", ButtonStyle.Box)]
    public void SetMaxValue(float maxValue)
    {
        SetValueRange(0, maxValue);
    }
}
