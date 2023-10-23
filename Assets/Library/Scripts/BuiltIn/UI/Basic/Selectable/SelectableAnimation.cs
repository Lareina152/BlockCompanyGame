using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class SelectableAnimation : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [LabelText("状态机参数：是否按下")]
    public string onPointerDownParameter = "OnPointerDown";
    [LabelText("状态机参数：是否高亮")]
    public string onPointerEnterParameter = "OnPointerEnter";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        animator.SetBool(onPointerDownParameter, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        animator.SetBool(onPointerDownParameter, false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool(onPointerEnterParameter, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool(onPointerEnterParameter, false);
    }
}
