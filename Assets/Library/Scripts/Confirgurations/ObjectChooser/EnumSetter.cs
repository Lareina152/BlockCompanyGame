using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Basis;
using ConfigurationBasis;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn, Description = "Enum Setter")]
public class EnumSetter<T> : ObjectChooser<T> where T : Enum
{
    protected override bool valueAlwaysPreviewed => true;

    #region GUI

    protected override ProbabilityItem AddProbabilityItemGUI()
    {
        if (ContainsAllEnumValues())
        {
            return null;
        }

        return new()
        {
            value = Enum.GetValues(typeof(T)).Cast<T>().Except(valueProbabilities.Select(item => item.value)).Choose(),
            ratio = 1
        };
    }

    [Button("添加所有可能")]
    [ShowIf(@"@isRandomValue && randomType == ""Weighted Select"" && ContainsAllEnumValues() == false")]
    private void AddAllEnumValues()
    {
        if (typeof(T).IsEnum == false)
        {
            return;
        }

        foreach (var enumValue in Enum.GetValues(typeof(T)))
        {
            if (valueProbabilities.Any(item => item.value.Equals(enumValue)))
            {
                continue;
            }

            valueProbabilities.Add(new()
            {
                value = (T)enumValue,
                ratio = 1
            });
        }

        OnValueProbabilitiesChanged();
    }

    private bool ContainsAllEnumValues()
    {
        return Enum.GetValues(typeof(T)).Cast<object>().
            All(enumValue => valueProbabilities.Any(item => item.value.Equals(enumValue)));
    }

    protected override string ValueToPreview(T value)
    {
        var labelText = typeof(T).GetField(value.ToString()).GetCustomAttribute<LabelTextAttribute>();
        return labelText == null ? value.ToString() : labelText.Text;
    }

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        if (typeof(T).IsEnum == false)
        {
            Note.note.Warning($"{nameof(T)}不是Enum类型不能放在EnumSetter里");
        }
    }

    #endregion

    public static implicit operator EnumSetter<T>(T enumValue)
    {
        return new()
        {
            isRandomValue = false,
            value = enumValue
        };
    }
}
