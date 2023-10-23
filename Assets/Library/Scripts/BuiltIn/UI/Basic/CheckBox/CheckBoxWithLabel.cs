using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheckBoxWithLabel : MonoBehaviour
{
    [LabelText("ֵΪTrueʱ�ı�ǩ")]
    [ChildGameObjectsOnly, Required]
    public TextMeshProUGUI labelTrue;

    [LabelText("ֵΪFalseʱ�ı�ǩ")]
    [ChildGameObjectsOnly, Required]
    public TextMeshProUGUI labelFalse;

    [LabelText("�л���")]
    [ChildGameObjectsOnly, Required]
    public Toggle toggle;

    [LabelText("��ֵ�ı�ʱ")]
    public UnityEvent<bool> onValueChanged;

    private void Awake()
    {
        toggle.onValueChanged.AddListener((currentValue) =>
        {
            onValueChanged?.Invoke(currentValue);

            labelTrue.SetActive(currentValue == true);
            labelFalse.SetActive(currentValue == false);
        });
    }

    public void SetLabelColor(Color color)
    {
        if (labelTrue != null)
        {
            labelTrue.color = color;
        }

        if (labelFalse != null)
        {
            labelFalse.color = color;
        }
    }
}
