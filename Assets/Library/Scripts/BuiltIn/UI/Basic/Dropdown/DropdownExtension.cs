using System.Collections;
using System.Collections.Generic;
using Basis;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Dropdown))]
public class DropdownExtension : MonoBehaviour, IPointerClickHandler, ICancelHandler
{
    [LabelText("箭头")]
    [PropertyTooltip("图片需朝下"), Required, ChildGameObjectsOnly]
    public RectTransform arrow;

    [LabelText("箭头旋转事件")]
    [MinValue(0), MaxValue(10)]
    public float arrowRotationDuration = 0.3f;

    private void Awake()
    {
        if (arrow != null)
        {
            arrow.SetAngle(-90);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        arrow.DOKill();
        arrow.DOLocalRotate(new(0, 0, 0), arrowRotationDuration);
    }

    public void OnCancel(BaseEventData eventData)
    {
        arrow.DOKill();
        arrow.DOLocalRotate(new(0, 0, -90), arrowRotationDuration);
    }
}
