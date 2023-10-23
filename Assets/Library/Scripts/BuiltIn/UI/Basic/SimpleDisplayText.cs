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
    [LabelText("�ı�����"), FoldoutGroup("����")]
    public TextMeshProUGUI text;


    [LabelText("��ǩ"), FoldoutGroup("��ǩ�͵�λ")]
    public TextWithTag label = "Label";

    [LabelText("��λ"), FoldoutGroup("��ǩ�͵�λ")]
    public TextWithTag unit = "";

    [LabelText("ֵ��ʽ"), FoldoutGroup("ֵ")]
    public TextTagFormat valueFormat = new();


    [LabelText("��ǩ��ֵ�ָ���"), FoldoutGroup("�ָ�")]
    public string sepLabelValue = "��";

    [LabelText("ֵ�����ֵ�ָ���"), FoldoutGroup("�ָ�")]
    public string sepValueMaxValue = "/";

    [LabelText("ֵ�͵�λ�ָ���"), FoldoutGroup("�ָ�")]
    public string sepValueUnit = "";

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        label ??= new();
        unit ??= new();
        valueFormat ??= new();
    }

    [Button("Ԥ��", ButtonSizes.Medium)]
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
