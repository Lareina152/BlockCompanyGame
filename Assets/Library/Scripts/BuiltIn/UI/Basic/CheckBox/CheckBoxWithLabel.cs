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
    [LabelText("值为True时的标签")]
    [ChildGameObjectsOnly, Required]
    public TextMeshProUGUI labelTrue;

    [LabelText("值为False时的标签")]
    [ChildGameObjectsOnly, Required]
    public TextMeshProUGUI labelFalse;

    [LabelText("切换框")]
    [ChildGameObjectsOnly, Required]
    public Toggle toggle;

    [LabelText("当值改变时")]
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
