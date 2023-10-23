using System;
using ConfigurationBasis;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Basis;

public class ValueHUDShortTextPrefab : UIParticlePrefab
{
    protected override Type requireType => typeof(ValueHUDShortText);

    [FoldoutGroup("��������")]
    [LabelText("������ɫ")]
    public ColorSetter textColor = new();

    [FoldoutGroup("��������")]
    [LabelText("�����С")]
    public IntegerSetter fontSize = new();

    [FoldoutGroup("��������")]
    [LabelText("С�������ʾ��λ")]
    public int decimalPlaces = 0;
}

public class ValueHUDShortText : UIParticleBase
{
    [LabelText("����"), BoxGroup("��������")]
    [SerializeField, Required]
    protected TextMeshProUGUI text;

    protected ValueHUDShortTextPrefab ObjectParticleShortTextPrefab => (ValueHUDShortTextPrefab)objectParticlePrefab;

    public ValueHUDShortText Set(string newText)
    {
        text.text = newText;
        text.fontSize = ObjectParticleShortTextPrefab.fontSize;
        text.color = ObjectParticleShortTextPrefab.textColor;
        return this;
    }

    public ValueHUDShortText Set(float value)
    {
        text.text = value.ToString(ObjectParticleShortTextPrefab.decimalPlaces);
        text.fontSize = ObjectParticleShortTextPrefab.fontSize.GetInterpolatedValue(value);
        text.color = ObjectParticleShortTextPrefab.textColor.GetInterpolatedValue(value);

        return this;
    }

    public new static ValueHUDShortText Create(string id, GameObject target, Vector2 tracingOffset = default)
    {
        return (ValueHUDShortText)UIParticleBase.Create(id, target, tracingOffset);
    }
}
