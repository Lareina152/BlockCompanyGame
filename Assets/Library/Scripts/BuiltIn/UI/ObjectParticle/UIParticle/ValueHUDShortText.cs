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

    [FoldoutGroup("文字设置")]
    [LabelText("文字颜色")]
    public ColorSetter textColor = new();

    [FoldoutGroup("文字设置")]
    [LabelText("字体大小")]
    public IntegerSetter fontSize = new();

    [FoldoutGroup("文字设置")]
    [LabelText("小数点后显示几位")]
    public int decimalPlaces = 0;
}

public class ValueHUDShortText : UIParticleBase
{
    [LabelText("文字"), BoxGroup("基本设置")]
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
