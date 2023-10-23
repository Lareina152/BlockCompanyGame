using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using ConfigurationBasis;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ValueDisplayUIBase : MonoBehaviourBase
{
    [LabelText("小数点后显示几位"), FoldoutGroup("基本")]
    [MinValue(0)]
    public int decimalPlaces = 0;

    [LabelText("值动画"), FoldoutGroup("基本")]
    public FloatTracer floatTracer = new();

    [LabelText("是否允许值超出范围"), FoldoutGroup("基本")]
    public bool allowValueOverflow = true;

    [LabelText("最大值"), FoldoutGroup("Debugging", Order = 100)] 
    [SerializeField] 
    protected float maxValue;
    [LabelText("最小值"), FoldoutGroup("Debugging")] 
    [SerializeField]
    protected float minValue;

    protected float percent => floatTracer.value.Percent(minValue, maxValue);

    protected string maxValueString => maxValue.ToString(decimalPlaces);
    protected string minValueString => minValue.ToString(decimalPlaces);
    protected string valueString => floatTracer.value.ToString(decimalPlaces);
    protected string percentString => percent.ToString(decimalPlaces);

    [LabelText("是否初始化"), FoldoutGroup("Debugging")]
    [ShowInInspector, DisplayAsString]
    protected bool hasInitialized = false;

    protected virtual void OnValueChanged()
    {

    }

    protected virtual void OnInit()
    {
        floatTracer.OnValueChanged += OnValueChanged;
        floatTracer.Init();
    }

    protected void SetValueRange([LabelText("最大值")] float minValue, [LabelText("最小值")] float maxValue)
    {
        Note.note.AssertIsAbove(maxValue, minValue, nameof(maxValue));

        this.minValue = minValue;
        this.maxValue = maxValue;

        if (hasInitialized == false)
        {
            OnInit();
            hasInitialized = true;
        }

        OnValueChanged();
    }

    [Button("设置值", ButtonStyle.Box)]
    public void SetValue([LabelText("值")] float value)
    {
        if (hasInitialized == false)
        {
            OnInit();
            hasInitialized = true;
        }

        if (allowValueOverflow == false)
        {
            value = value.Clamp(minValue, maxValue);
        }

        floatTracer.SetValue(value);
    }
}
